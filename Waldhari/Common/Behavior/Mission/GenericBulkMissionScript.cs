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
    public abstract class GenericBulkMissionScript : AbstractMissionScript
    {
        // Scene
        private WBlip _deliveryWBlip;
        private List<PedActingScript> _wholesalerActingScripts;
        private WVehicle _van;

        protected int _amount;
        protected int _price;

        protected abstract int Amount { get; }
        protected abstract int PriceByUnit { get; }
        protected abstract void DeductAmount(int amount);
        protected abstract WPosition Parking { get; }
        protected abstract void ShowStartedMessage();

        protected GenericBulkMissionScript(string name) 
            : base(name, true, "bulk_success") {}

        protected override void StartComplement()
        {
            _amount = Amount;
            if (_amount <= 0)
                throw new MissionException("no_product");
            DeductAmount(_amount);

            _price = _amount * PriceByUnit;
            
            ShowStartedMessage();
        }

        protected override bool OnTickComplement()
        {
            if (_wholesalerActingScripts[0]?.WPed?.Ped == null || _wholesalerActingScripts[0].WPed.Ped.IsDead)
                throw new MissionException("bulk_fail_wholesaler_dead");

            if (_van?.Vehicle == null || _van.Vehicle.IsConsideredDestroyed)
                throw new MissionException("fail_vehicle_destroyed");

            return true;
        }

        protected override List<string> EndComplement()
        {
            SoundHelper.PlayPayment();
            Game.Player.Money += _price;
            Game.DoAutoSave();

            return new List<string> { _price.ToString() };
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
                MessageKey = "step_enter_vehicle",
                Action = () =>
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
                MessageKey = "bulk_step_rendezvous",
                Action = () =>
                {
                    _van.RemoveMissionDestination();
                    _deliveryWBlip.Create();
                    MarkerHelper.DrawGroundMarkerOnBlip(_deliveryWBlip);
                },
                CompletionCondition = 
                    () => WPositionHelper.IsNear(Game.Player.Character.Position, _deliveryWBlip.Position, 10)
            };
        }

        private Step GetStepGetOutVehicle()
        {
            return new Step
            {
                Name = "GetOutVehicle",
                MessageKey = "step_getout_vehicle",
                Action = () => _deliveryWBlip.Remove(),
                CompletionCondition = 
                    () => !Game.Player.Character.IsInVehicle(_van.Vehicle)
            };
        }

        private Step GetStepPayment()
        {
            return new Step
            {
                Name = "Payment",
                MessageKey = "bulk_step_payment",
                Action = () =>
                {
                    _wholesalerActingScripts[0].WPed.MakeMissionDestination("wholesaler");
                },
                CompletionCondition = 
                    () => !_wholesalerActingScripts[0].WPed.Ped.IsInCombat &&
                          WPositionHelper.IsNear(Game.Player.Character.Position, _wholesalerActingScripts[0].WPed.Ped.Position, 2),
                CompletionAction = 
                    () => _wholesalerActingScripts[0].WPed.RemoveMissionDestination()
            };
        }

        protected override void CreateScene()
        {
            var randomPosition = WPositionHelper.GetRandomMissionWithVehiclePosition();
            var gang = WGroupHelper.GetRandomGang();

            _wholesalerActingScripts = WSceneHelper.CreateTransactionScene(randomPosition, gang);

            foreach (var pedScript in _wholesalerActingScripts)
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
                InitialPosition = Parking
            };
            _van.Create();

            _deliveryWBlip = WBlipHelper.GetMission("delivery");
            _deliveryWBlip.Position = randomPosition.VehiclePosition.Position;
        }

        protected override void CleanScene()
        {
            if (_wholesalerActingScripts?.Count > 0)
            {
                foreach (var pedScript in _wholesalerActingScripts)
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
