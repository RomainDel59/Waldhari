using System;
using System.Collections.Generic;
using GTA;
using Waldhari.Behavior.Mission;
using Waldhari.Behavior.Ped;
using Waldhari.Common.Entities;
using Waldhari.Common.Entities.Helpers;
using Waldhari.Common.Exceptions;
using Waldhari.Common.Misc;
using Waldhari.Common.UI;

namespace Waldhari.MethLab.Missions
{
    [ScriptAttributes(NoDefaultInstance = true)]
    public class DealMissionScript : AbstractMissionScript
    {
        // Scene
        private PedActingScript _clientScript;

        private int _amountToDeal;
        private int _priceToDeal;

        public DealMissionScript() : base("MethLabDealMission", true, "methlab_deal_success")
        {
        }

        protected override bool StartComplement(string arg)
        {
            if (MethLabSave.Instance.Product <= 0)
            {
                NotificationHelper.ShowFailure("methlab_deal_no_product");
                return false;
            }

            if (WPositionHelper.IsNear(Game.Player.Character.Position,ManufactureMissionScript.AnimationPosition,5))
            {
                NotificationHelper.ShowFailure("methlab_deal_not_close_enough");
                return false;
            }

            _amountToDeal = RandomHelper.Next(1, MethLabOptions.Instance.DealMaxGramsPerPack + 1);
            _amountToDeal = Math.Min(_amountToDeal, MethLabSave.Instance.Product);
            _priceToDeal = _amountToDeal * GetPricePerGram();

            NotificationHelper.ShowFromRon("methlab_deal_started_ron",
                new List<string> { _amountToDeal.ToString(), _priceToDeal.ToString() });

            MethLabSave.Instance.Product -= _amountToDeal;
            MethLabSave.Instance.Save();

            return true;
        }

        protected override void OnTickComplement()
        {
            if (_clientScript == null || _clientScript.WPed == null || _clientScript.WPed.Ped.IsDead) throw new MissionException("methlab_deal_fail_client_dead");
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
                MessageKey = "methlab_deal_rendezvous",
                Action = () => { _clientScript.WPed.MakeMissionDestination("methlab_deal_client"); },
                CompletionCondition = () =>
                    WPositionHelper.IsNear(Game.Player.Character.Position,_clientScript.WPed.Ped.Position,25)
            };
        }

        private Step GetStepPayment()
        {
            return new Step
            {
                Name = "Payment",
                MessageKey = "methlab_deal_payment",
                Action = () =>
                {
                    _clientScript.WPed.MakeMissionDestination("supply_supplier");
                },
                CompletionCondition = () =>
                    !_clientScript.WPed.Ped.IsInCombat &&
                    WPositionHelper.IsNear(Game.Player.Character.Position,_clientScript.WPed.Ped.Position, 2)
            };
        }

        private static int GetPricePerGram()
        {
            return
                RandomHelper.Next(MethLabOptions.Instance.DealMinPrice, MethLabOptions.Instance.DealMaxPrice + 1);
        }

        protected override void CreateScene()
        {
            var client = new WPed
            {
                PedHash = PedHash.Rurmeth01AMM,
                InitialPosition = new WPosition
                {
                    Position = MethLabHelper.GetRandomPosition()
                    //todo: make MethLabPositions returns WPosition
                },
                Scenario = "WORLD_HUMAN_SMOKING"
            };
            client.Create();
            client.AddWeapon(WeaponsHelper.GetRandomGangWeapon());
            
            _clientScript = InstantiateScript<PedActingScript>();
            _clientScript.WPed = client;
        }

        protected override void CleanScene()
        {
            _clientScript?.Abort();
        }
    }
}