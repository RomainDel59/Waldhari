using System.Collections.Generic;
using GTA;
using Waldhari.Common.Behavior.Ped;
using Waldhari.Common.Entities;
using Waldhari.Common.Entities.Helpers;
using Waldhari.Common.Exceptions;
using Waldhari.Common.UI;

namespace Waldhari.Common.Behavior.Mission
{
    [ScriptAttributes(NoDefaultInstance = true)]
    public abstract class GenericSupplyMissionScript : AbstractMissionScript
    {
        // Scene
        private WBlip _deliveryWBlip;
        private PedActingScript _sellerScript;
        private WVehicle _van;

        private int _amountToSupply;
        private int _costToSupply;
        
        protected abstract int SupplyAmount { get; }
        protected abstract int SupplyCost { get; }
        protected abstract string StepDriveMessageKey { get; }
        protected abstract string DestinationMessageKey { get; }
        protected abstract WPosition Parking { get; }
        protected abstract void AddSupply(int amount);
        protected abstract void ShowStartedMessage();

        protected GenericSupplyMissionScript(string name) : 
            base(name, true, "supply_success") {}

        protected override void StartComplement()
        {
            _amountToSupply = SupplyAmount;
            _costToSupply = _amountToSupply * SupplyCost;

            if (Game.Player.Money < _costToSupply)
                throw new MissionException("supply_no_money");

            ShowStartedMessage();
        }

        protected override bool OnTickComplement()
        {
            if (_sellerScript?.WPed?.Ped == null || _sellerScript.WPed.Ped.IsDead)
                throw new MissionException("supply_fail_supplier_dead");

            if (_van?.Vehicle == null || _van.Vehicle.IsConsideredDestroyed)
                throw new MissionException("supply_fail_vehicle_destroyed");

            return true;
        }

        protected override List<string> EndComplement()
        {
            AddSupply(_amountToSupply);
            
            return new List<string> { _amountToSupply.ToString() };
        }

        protected override void FailComplement()
        {
            // Nothing specific to handle
        }

        protected override void SetupSteps()
        {
            AddWantedStep();
            AddRivalStep();
            AddStep(GetStepRendezvous(), false);
            AddStep(GetStepPayment());
            AddStep(GetStepDrive(), false);
            AddStep(GetStepOut());
        }

        private Step GetStepRendezvous()
        {
            return new Step
            {
                Name = "Rendezvous",
                MessageKey = "supply_step_rendezvous",
                Action = () =>
                {
                    _sellerScript.WPed.MakeMissionDestination("supplier");
                },
                CompletionCondition =
                    () => WPositionHelper.IsNear(Game.Player.Character.Position, _sellerScript.WPed.Ped.Position, 25)
            };
        }

        private Step GetStepPayment()
        {
            return new Step
            {
                Name = "Payment",
                MessageKey = "supply_step_payment",
                Action = () =>
                {
                    _sellerScript.WPed.MakeMissionDestination("supplier");
                },
                CompletionCondition =
                    () => !_sellerScript.WPed.Ped.IsInCombat &&
                          WPositionHelper.IsNear(Game.Player.Character.Position, _sellerScript.WPed.Ped.Position, 2),
                CompletionAction = () =>
                {
                    SoundHelper.PlayPayment();
                    Game.Player.Money -= _costToSupply;
                    Game.DoAutoSave();
                }
            };
        }

        private Step GetStepDrive()
        {
            return new Step
            {
                Name = "Drive",
                MessageKey = StepDriveMessageKey,
                Action = () =>
                {
                    _sellerScript.WPed.RemoveMissionDestination();

                    if (!Game.Player.Character.IsInVehicle(_van.Vehicle))
                    {
                        _deliveryWBlip.Remove();
                        _van.MakeMissionDestination("vehicle_van");
                    }
                    else
                    {
                        _van.RemoveMissionDestination();
                        _deliveryWBlip.Create();
                        MarkerHelper.DrawGroundMarkerOnBlip(_deliveryWBlip);
                    }
                },
                CompletionCondition =
                    () => WPositionHelper.IsNear(Game.Player.Character.Position, _deliveryWBlip.Position, 10)
            };
        }

        private Step GetStepOut()
        {
            return new Step
            {
                Name = "Out",
                MessageKey = "supply_step_out",
                Action = () =>
                {
                    _deliveryWBlip.Remove();
                },
                CompletionCondition =
                    () => !Game.Player.Character.IsInVehicle(_van.Vehicle)
            };
        }

        protected override void CreateScene()
        {
            var randomPosition = WPositionHelper.GetRandomMissionWithVehiclePosition();

            // todo: random pedhash
            var seller = new WPed
            {
                PedHash = PedHash.CartelGuards01GMM,
                InitialPosition = randomPosition.PedPositions[0],
                Scenario = "WORLD_HUMAN_SMOKING"
            };
            seller.Create();
            seller.AddWeapon(WeaponsHelper.GetRandomGangWeapon());

            _sellerScript = InstantiateScript<PedActingScript>();
            _sellerScript.WPed = seller;

            // todo: random vehiclehash
            _van = new WVehicle
            {
                VehicleHash = VehicleHash.Burrito,
                InitialPosition = randomPosition.VehiclePosition
            };
            _van.Create();

            _deliveryWBlip = WBlipHelper.GetMission(DestinationMessageKey);
            _deliveryWBlip.Position = Parking.Position;
        }

        protected override void CleanScene()
        {
            _sellerScript?.WPed?.Ped?.MarkAsNoLongerNeeded();
            _sellerScript?.Abort();

            if (_van != null)
            {
                _van.WBlip?.Remove();
                _van.Vehicle?.MarkAsNoLongerNeeded();
                if (_van.Vehicle != null)
                {
                    if (Game.Player.Character.IsInVehicle(_van.Vehicle))
                    {
                        Game.Player.Character.Task.LeaveVehicle();
                    }
                    _van.Vehicle.IsConsideredDestroyed = true;
                }
            }

            _deliveryWBlip?.Remove();
        }
    }
}