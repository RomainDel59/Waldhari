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
    public abstract class GenericBulkMissionScript : AbstractMissionScript
    {
        // Scene
        private WBlip _deliveryWBlip;
        private List<PedActingScript> _wholesalerActingScripts;
        private WVehicle _van;

        private int _amountToDeliver;
        private int _priceToDeliver;

        protected abstract int DeliverAmount { get; }
        protected abstract int DeliverPrice { get; }
        protected abstract void DeductAmount(int amount);
        protected abstract string StartMessageKey { get; }
        protected abstract string SuccessMessageKey { get; }
        protected abstract string DeliveryMessageKey { get; }
        protected abstract WPosition DeliveryPosition { get; }
        protected abstract void OnDeliverySuccess(int amount, int totalPrice);
        protected abstract void ShowStartedMessage();

        protected GenericBulkMissionScript(string name) 
            : base(name, true, "bulk_success") {}

        protected override void StartComplement()
        {
            _amountToDeliver = DeliverAmount;
            if (_amountToDeliver <= 0)
                throw new MissionException("no_product");

            _priceToDeliver = _amountToDeliver * DeliverPrice;
            DeductAmount(_amountToDeliver);
            
            ShowStartedMessage();
        }

        protected override bool OnTickComplement()
        {
            if (_wholesalerActingScripts[0]?.WPed?.Ped == null || _wholesalerActingScripts[0].WPed.Ped.IsDead)
                throw new MissionException("bulk_fail_wholesaler_dead");

            if (_van?.Vehicle == null || _van.Vehicle.IsConsideredDestroyed)
                throw new MissionException("bulk_fail_vehicle_destroyed");

            return true;
        }

        protected override List<string> EndComplement()
        {
            OnDeliverySuccess(_amountToDeliver, _priceToDeliver);

            return new List<string> { _priceToDeliver.ToString() };
        }

        protected override void FailComplement()
        {
            // Handle cleanup on failure if needed
        }

        protected override void SetupSteps()
        {
            AddWantedStep();
            AddRivalStep();
            AddStep(CreateEnterVehicleStep(), false);
            AddStep(CreateRendezvousStep());
            AddStep(CreateExitVehicleStep());
            AddStep(CreateDeliveryStep(), false);
        }

        private Step CreateEnterVehicleStep()
        {
            return new Step
            {
                Name = "EnterVehicle",
                MessageKey = "delivery_step_enter_vehicle",
                Action = () =>
                {
                    _deliveryWBlip.Remove();
                    _deliveryVehicle.MakeMissionDestination("delivery_vehicle");
                },
                CompletionCondition = 
                    () => Game.Player.Character.IsInVehicle(_deliveryVehicle.Vehicle)
            };
        }

        private Step CreateRendezvousStep()
        {
            return new Step
            {
                Name = "Rendezvous",
                MessageKey = DeliveryMessageKey,
                Action = () =>
                {
                    _deliveryVehicle.RemoveMissionDestination();
                    _deliveryWBlip.Create();
                    MarkerHelper.DrawGroundMarkerOnBlip(_deliveryWBlip);
                },
                CompletionCondition = 
                    () => WPositionHelper.IsNear(Game.Player.Character.Position, _deliveryWBlip.Position, 10)
            };
        }

        private Step CreateExitVehicleStep()
        {
            return new Step
            {
                Name = "ExitVehicle",
                MessageKey = "delivery_step_exit_vehicle",
                Action = () => _deliveryWBlip.Remove(),
                CompletionCondition = 
                    () => !Game.Player.Character.IsInVehicle(_deliveryVehicle.Vehicle)
            };
        }

        private Step CreateDeliveryStep()
        {
            return new Step
            {
                Name = "Delivery",
                MessageKey = "delivery_step_complete",
                Action = () =>
                {
                    _deliveryAgentScript.WPed.MakeMissionDestination("delivery_agent");
                },
                CompletionCondition = 
                    () => !_deliveryAgentScript.IsInCombat() &&
                          WPositionHelper.IsNear(Game.Player.Character.Position, _deliveryAgentScript.WPed.Ped.Position, 2)
            };
        }

        protected override void CreateScene()
        {
            _deliveryAgentScript = CreateDeliveryAgent();
            _deliveryWBlip = CreateDeliveryBlip();
            _deliveryVehicle = CreateDeliveryVehicle();
        }

        protected override void CleanScene()
        {
            _deliveryAgentScript?.WPed?.Ped?.MarkAsNoLongerNeeded();
            _deliveryAgentScript?.Abort();

            if (_deliveryVehicle != null)
            {
                _deliveryVehicle.WBlip?.Remove();
                _deliveryVehicle.Vehicle?.MarkAsNoLongerNeeded();
                if (_deliveryVehicle.Vehicle != null)
                    _deliveryVehicle.Vehicle.IsConsideredDestroyed = true;
            }

            _deliveryWBlip?.Remove();
        }

        protected abstract PedActingScript CreateDeliveryAgent();
        protected abstract WBlip CreateDeliveryBlip();
        protected abstract WVehicle CreateDeliveryVehicle();
    }
}
