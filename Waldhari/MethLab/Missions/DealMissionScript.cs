using System;
using System.Collections.Generic;
using GTA;
using Waldhari.Common.Behavior.Mission;
using Waldhari.Common.Behavior.Ped;
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
        
        private WBlip _wProductBlip;

        private int _amountToDeal;
        private int _priceToDeal;

        public DealMissionScript() : base("MethLabDealMission", true, "methlab_deal_success")
        {
        }

        protected override void StartComplement()
        {
            if (MethLabSave.Instance.Product <= 0) 
                throw new MissionException("methlab_deal_no_product");

            _amountToDeal = RandomHelper.Next(MethLabOptions.Instance.DealMinGramsPerPack, MethLabOptions.Instance.DealMaxGramsPerPack + 1);
            _amountToDeal = Math.Min(_amountToDeal, MethLabSave.Instance.Product);
            _priceToDeal = _amountToDeal * GetPricePerGram();

            NotificationHelper.ShowFromRon("methlab_deal_started",
                new List<string> { _amountToDeal.ToString(), _priceToDeal.ToString() });

            MethLabSave.Instance.Product -= _amountToDeal;
            MethLabSave.Instance.Save();
        }

        protected override bool OnTickComplement()
        {
            if (_clientScript == null || _clientScript.WPed == null || _clientScript.WPed.Ped.IsDead) throw new MissionException("methlab_deal_fail_client_dead");

            return true;
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
            AddStep(GetStepProduct(), false);
            AddStep(GetStepRendezvous(), false);
            AddStep(GetStepPayment(), false);
        }

        private Step GetStepProduct()
        {
            return new Step
            {
                Name = "Product",
                MessageKey = "methlab_deal_get_product",
                Action = () =>
                {
                    _wProductBlip.Create();
                    MarkerHelper.DrawGroundMarkerOnBlip(_wProductBlip, 2);
                },
                CompletionCondition = () =>
                    WPositionHelper.IsNear(Game.Player.Character.Position,_wProductBlip.Position,2),
                CompletionAction = SoundHelper.PlayTake
            };
        }

        private Step GetStepRendezvous()
        {
            return new Step
            {
                Name = "Rendezvous",
                MessageKey = "methlab_deal_rendezvous",
                Action = () =>
                {
                    _wProductBlip.Remove();
                    _clientScript.WPed.MakeMissionDestination("methlab_deal_client");
                },
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
            _wProductBlip = WBlipHelper.GetMission("methlab_deal_product");
            _wProductBlip.Position = MethLabHelper.Positions.Storage.Position;
            
            var client = new WPed
            {
                PedHash = PedHash.Rurmeth01AMM,
                InitialPosition = WPositionHelper.GetRandomAlonePedPosition(),
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