using System.Collections.Generic;
using GTA;
using GTA.Math;
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
    public class BulkMissionScript : AbstractMissionScript
    {
        // Scene
        private WBlip _deliveryWBlip;
        private PedActingScript _wholesalerScript;
        private WVehicle _van;

        private int _amountToDeal;
        private int _priceToDeal;

        public BulkMissionScript() : base("MethLabBulkMission", true, "methlab_bulk_success")
        {
        }

        protected override void StartComplement()
        {
            if (MethLabSave.Instance.Product <= 0) 
                throw new MissionException("methlab_deal_no_product");

            _amountToDeal = MethLabSave.Instance.Product;
            _priceToDeal = _amountToDeal * GetPricePerGram();

            NotificationHelper.ShowFromRon("methlab_bulk_started",
                new List<string> { _priceToDeal.ToString() });

            MethLabSave.Instance.Product -= _amountToDeal;
            MethLabSave.Instance.Save();
        }

        protected override bool OnTickComplement()
        {
            if (_wholesalerScript.WPed == null || _wholesalerScript.WPed.Ped.IsDead) throw new MissionException("methlab_bulk_fail_wholesaler_dead");

            if (_van == null || _van.Vehicle == null || _van.Vehicle.IsConsideredDestroyed) throw new MissionException("methlab_bulk_fail_vehicle_destroyed");

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
            AddStep(GetStepEnterVehicle(), false);
            AddStep(GetStepRendezvous());
            AddStep(GetStepGetOutVehicle());
            AddStep(GetStepPayment(), false);
        }

        private Step GetStepEnterVehicle()
        {
            return new Step
            {
                Name = "EnterVehicle",
                MessageKey = "methlab_bulk_in",
                Action =
                    () =>
                    {
                        _deliveryWBlip.Remove();
                        _van.MakeMissionDestination("vehicle_van");
                    },
                CompletionCondition =
                    () => Game.Player.Character.IsInVehicle(_van.Vehicle)
            };
        }

        private Step GetStepRendezvous()
        {
            return new Step
            {
                Name = "Rendezvous",
                MessageKey = "methlab_bulk_rendezvous",
                Action =
                    () =>
                    {
                        _van.RemoveMissionDestination();
                        _deliveryWBlip.Create();
                        MarkerHelper.DrawGroundMarkerOnBlip(_deliveryWBlip);
                    },
                CompletionCondition =
                    () => WPositionHelper.IsNear(Game.Player.Character.Position,_deliveryWBlip.Position,10)
            };
        }

        private Step GetStepGetOutVehicle()
        {
            return new Step
            {
                Name = "GetOutVehicle",
                MessageKey = "methlab_bulk_out",
                Action =
                    () => { _deliveryWBlip.Remove(); },
                CompletionCondition =
                    () => !Game.Player.Character.IsInVehicle(_van.Vehicle)
            };
        }

        private Step GetStepPayment()
        {
            return new Step
            {
                Name = "Payment",
                MessageKey = "methlab_bulk_payment",
                Action = () =>
                {
                    _wholesalerScript.WPed.MakeMissionDestination("methlab_bulk_wholesaler");
                },
                CompletionCondition =
                    () => !_wholesalerScript.IsInCombat() &&
                          WPositionHelper.IsNear(Game.Player.Character.Position, _wholesalerScript.WPed.Ped.Position, 2)
            };
        }

        protected override void CreateScene()
        {
            var randomPosition = WPositionHelper.GetRandomMissionWithVehiclePosition();
            
            var wholesaler = new WPed
            {
                PedHash = PedHash.CartelGuards01GMM,
                InitialPosition = randomPosition.PedPositions[0],
                Scenario = "WORLD_HUMAN_SMOKING"
            };
            wholesaler.Create();
            wholesaler.AddWeapon(WeaponsHelper.GetRandomGangWeapon());
            
            _wholesalerScript = InstantiateScript<PedActingScript>();
            _wholesalerScript.WPed = wholesaler;

            _deliveryWBlip = WBlipHelper.GetMission("methlab_bulk_delivery");
            _deliveryWBlip.Position = randomPosition.VehiclePosition.Position;


            _van = new WVehicle
            {
                VehicleHash = VehicleHash.Burrito,
                InitialPosition = new WPosition{
                    Position = new Vector3(1376.08f, 3619.386f, 34.89184f),
                    Rotation = new Vector3(0, 0, 173.9757f),
                    Heading = 173.9757f
                }
            };
            _van.Create();
        }

        protected override void CleanScene()
        {
            _wholesalerScript?.WPed?.Ped?.MarkAsNoLongerNeeded();
            _wholesalerScript?.Abort();

            if (_van != null)
            {
                _van.WBlip?.Remove();
                _van.Vehicle?.MarkAsNoLongerNeeded();
                if(_van.Vehicle != null) _van.Vehicle.IsConsideredDestroyed = true;
            }

            _deliveryWBlip?.Remove();
        }

        private static int GetPricePerGram()
        {
            return
                RandomHelper.Next(MethLabOptions.Instance.BulkMinPrice, MethLabOptions.Instance.BulkMaxPrice + 1);
        }
    }
}