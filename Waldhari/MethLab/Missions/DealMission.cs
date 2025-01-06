using System;
using System.Collections.Generic;
using GTA;

namespace GTAVMods.Missions
{
    public class DealMission : AbstractMission
    {
        // Scene
        private WPed _client;

        private int _amountToDeal;
        private int _priceToDeal;

        public DealMission() : base("DealMission", true, "deal_success")
        {
        }

        protected override bool StartComplement(string arg)
        {
            if (ModSave.Instance.Product <= 0)
            {
                NotificationHelper.ShowFail("deal_no_product");
                return false;
            }

            if (Game.Player.Character.Position.DistanceTo(ManufactureMission.AnimationPosition) > 5)
            {
                NotificationHelper.ShowFail("deal_not_close_enough");
                return false;
            }

            _amountToDeal = Math.Min(ModOptions.Instance.DealMaxGramsPerPack, ModSave.Instance.Product);
            _priceToDeal = _amountToDeal * GetPricePerGram();

            NotificationHelper.ShowFromRon("deal_started_ron",
                new List<string> { _amountToDeal.ToString(), _priceToDeal.ToString() });

            ModSave.Instance.Product -= _amountToDeal;
            ModSave.Instance.Save();

            return true;
        }

        protected override void UpdateComplement()
        {
            if (_client == null || _client.GtaPed.IsDead) throw new MissionException("deal_fail_client_dead");
        }

        protected override List<string> EndComplement()
        {
            Game.Player.Money += _priceToDeal;
            Game.DoAutoSave();

            return new List<string> { _priceToDeal.ToString() };
        }

        protected override void FailComplement()
        {
            // Nothing
        }

        protected override void SetupSteps()
        {
            AddWantedStep();
            AddRivalStep();
            AddStep(GetStepRendezvous(), false);
            AddStep(GetStepPayment(), false);
        }

        private Step GetStepRendezvous()
        {
            return new Step
            {
                Name = "Rendezvous",
                MessageKey = "deal_rendezvous",
                Action = () => { _client.AttachMissionBlip("deal_client"); },
                CompletionCondition = () =>
                    Game.Player.Character.Position.DistanceTo(_client.GtaPed.Position) <= 25
            };
        }

        private Step GetStepPayment()
        {
            return new Step
            {
                Name = "Payment",
                MessageKey = "deal_payment",
                Action = () =>
                {
                    _client.AttachMissionBlip("supply_supplier");
                    _client.AttachMissionMarker();
                },
                CompletionCondition = () =>
                    !_client.GtaPed.IsInCombat &&
                    Game.Player.Character.IsInRange(_client.GtaPed.Position, 2.0f)
            };
        }

        private static int GetPricePerGram()
        {
            return
                RandomHelper.Next(ModOptions.Instance.DealMinPrice, ModOptions.Instance.DealMaxPrice + 1) *
                (1 + (ModSave.Instance.Blue ? ModOptions.Instance.BluePercent / 100 : 0));
        }

        protected override void CreateScene()
        {
            _client = new WPed(
                PedHash.Rurmeth01AMM,
                MethLabPositions.GetRandom()
            );
            _client.GiveWeapons();
            _client.PlayScenario("WORLD_HUMAN_SMOKING");
        }

        protected override void CleanScene()
        {
            if (_client != null)
            {
                _client.RemoveBlip();

                if (_client.GtaPed != null)
                {
                    _client.GtaPed.MarkAsNoLongerNeeded();
                }
            }
        }
    }
}