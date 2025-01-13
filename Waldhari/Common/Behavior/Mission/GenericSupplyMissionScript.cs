using System.Collections.Generic;
using GTA;
using Waldhari.Common.Behavior.Ped;
using Waldhari.Common.Entities;
using Waldhari.Common.Entities.Helpers;
using Waldhari.Common.Exceptions;
using Waldhari.Common.Files;
using Waldhari.Common.UI;

namespace Waldhari.Common.Behavior.Mission
{
    [ScriptAttributes(NoDefaultInstance = true)]
    public abstract class GenericSupplyMissionScript : AbstractMissionScript
    {
        // Scene
        private WBlip _deliveryWBlip;
        private List<PedActingScript> _supplierActingScripts;
        private WVehicle _van;

        private int _amount;
        private int _cost;
        
        protected abstract int Amount { get; }
        protected abstract int CostByUnit { get; }
        protected abstract string StepDriveMessageKey { get; }
        protected abstract string DestinationMessageKey { get; }
        protected abstract WPosition Parking { get; }
        protected abstract void AddSupply(int amount);
        protected abstract void ShowStartedMessage();

        protected GenericSupplyMissionScript(string name) : 
            base(name, true, "supply_success") {}

        protected override void StartComplement()
        {
            _amount = Amount;
            _cost = _amount * CostByUnit;

            if (Game.Player.Money < _cost)
                throw new MissionException("supply_no_money");

            ShowStartedMessage();
        }

        protected override bool OnTickComplement()
        {
            if (_supplierActingScripts[0]?.WPed?.Ped == null || _supplierActingScripts[0].WPed.Ped.IsDead)
                throw new MissionException("supply_fail_supplier_dead");

            if (_van?.Vehicle == null || _van.Vehicle.IsConsideredDestroyed)
                throw new MissionException("fail_vehicle_destroyed");

            return true;
        }

        protected override List<string> EndComplement()
        {
            AddSupply(_amount);
            
            return new List<string> { _amount.ToString() };
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
                    _supplierActingScripts[0].WPed.MakeMissionDestination("supplier");
                },
                CompletionCondition =
                    () => WPositionHelper.IsNear(Game.Player.Character.Position, _supplierActingScripts[0].WPed.Ped.Position, 25)
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
                    _supplierActingScripts[0].WPed.MakeMissionDestination("supplier");
                },
                CompletionCondition =
                    () => !_supplierActingScripts[0].WPed.Ped.IsInCombat &&
                          WPositionHelper.IsNear(Game.Player.Character.Position, _supplierActingScripts[0].WPed.Ped.Position, 2),
                CompletionAction = () =>
                {
                    SoundHelper.PlayPayment();
                    Game.Player.Money -= _cost;
                    Game.DoAutoSave();
                    
                    _supplierActingScripts[0].WPed.RemoveMissionDestination();
                    // TODO: (immersive feature) make peds go away
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

                    // todo: add step "in"
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
                MessageKey = "step_getout_vehicle",
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
            var gang = WGroupHelper.GetRandomGang();

            _supplierActingScripts = WSceneHelper.CreateTransactionScene(randomPosition,gang);

            foreach (var pedScript in _supplierActingScripts)
            {
                if (pedScript?.WPed?.Ped == null)
                {
                    Logger.Warning($"Ped is not present at CreateScene in {Name}");
                    continue;
                }
                pedScript.WPed.Ped.RelationshipGroup = Game.Player.Character.RelationshipGroup;
                Logger.Debug("Ped added to player relationship group");
            }
            
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
            if (_supplierActingScripts?.Count > 0)
            {
                foreach (var pedScript in _supplierActingScripts)
                {
                    pedScript?.WPed?.Ped?.MarkAsNoLongerNeeded();
                    pedScript?.Abort();
                }
            }

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