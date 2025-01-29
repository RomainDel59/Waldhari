using GTA;
using Waldhari.Common.Behavior.Ped;
using Waldhari.Common.Entities;
using Waldhari.Common.Entities.Helpers;
using Waldhari.Common.Exceptions;
using Waldhari.Common.UI;

namespace Waldhari.Common.Behavior.Mission
{
    [ScriptAttributes(NoDefaultInstance = true)]
    public abstract class GenericPickUpPedMissionScript : AbstractMissionScript
    {
        // Scene
        private PedActingScript _pedActingScript;
        private WBlip _destinationBlip;
        
        protected abstract WPosition Destination { get; }
        protected abstract void ShowStartedMessage();
        protected abstract string PedMessageKey { get; }
        protected abstract string FailPedDeadMessageKey { get; }
        protected abstract string RendezvousMessageKey { get; }
        protected abstract string WaitMessageKey { get; }
        protected abstract string DriveMessageKey { get; }
        protected abstract PedHash PedHash { get; }
        protected abstract string DestinationMessageKey { get; }

        protected abstract void SendPed(PedActingScript script);
        
        
        protected GenericPickUpPedMissionScript(string name, string successKey)
            : base(name, true, successKey) {}

        protected override void StartComplement()
        {
            ShowStartedMessage();
        }

        protected override bool OnTickComplement()
        {
            if (_pedActingScript?.WPed?.Ped == null || _pedActingScript.WPed.Ped.IsDead)
                throw new MissionException(FailPedDeadMessageKey);

            return true;
        }
        
        protected override void FailComplement()
        {
            _pedActingScript?.WPed?.Ped?.MarkAsNoLongerNeeded();
            _pedActingScript?.Abort();
        }

        protected override void SetupSteps()
        {
            AddWantedStep();
            AddStep(GetStepRendezvous(), false);
            AddStep(GetStepVehicle());
            AddStep(GetStepWait());
            AddStep(GetStepDrive());
        }
        
        private Step GetStepRendezvous()
        {
            return new Step
            {
                Name = "Rendezvous",
                MessageKey = RendezvousMessageKey,
                Action = () =>
                {
                    if(_pedActingScript.WPed.Ped.IsInVehicle()) _pedActingScript.WPed.Ped.Task.LeaveVehicle();
                    _pedActingScript.WPed.MakeMissionDestination(PedMessageKey);
                },
                CompletionCondition =
                    () => WPositionHelper.IsNearPlayer(_pedActingScript.WPed.Ped.Position, 25),
                CompletionAction = 
                    () => _pedActingScript.WPed.RemoveMissionDestination()
            };
        }

        private Step GetStepVehicle()
        {
            return new Step
            {
                Name = "Vehicle",
                MessageKey = "pickupped_step_vehicle",
                Action = () => { },
                CompletionCondition = 
                    () => Game.Player.Character.IsInVehicle(),
                CompletionAction = () =>
                {
                    _pedActingScript.StopActing();
                    _pedActingScript.WPed.Ped.Task.EnterVehicle(
                        Game.Player.Character.CurrentVehicle, 
                        VehicleSeat.Passenger, 
                        -1, 
                        //speed 2 = run
                        2);
                }
            };
        }

        private Step GetStepWait()
        {
            return new Step
            {
                Name = "Wait",
                MessageKey = WaitMessageKey,
                Action = () => { },
                CompletionCondition =
                    () => _pedActingScript.WPed.Ped.CurrentVehicle == Game.Player.Character.CurrentVehicle
            };
        }

        private Step GetStepDrive()
        {
            return new Step
            {
                Name = "Drive",
                MessageKey = DriveMessageKey,
                Action = () =>
                {
                    _destinationBlip.Create();
                    MarkerHelper.DrawGroundMarkerOnBlip(_destinationBlip);
                },
                CompletionCondition = 
                    () => WPositionHelper.IsNearPlayer(_destinationBlip.Position, 10) &&
                          Game.Player.Character.CurrentVehicle.Speed == 0,
                CompletionAction = 
                    () => SendPed(_pedActingScript)
            };
        }

        protected override void CreateScene()
        {
            var randomPosition = WPositionHelper.GetRandomAlonePedPosition();

            var ped = new WPed
            {
                PedHash = PedHash,
                InitialPosition = randomPosition,
                Scenario = WPed.PedScenario.Smoking
            };
            ped.Create();
            ped.AddWeapon(WeaponsHelper.GetRandomGangVehicleWeapon());
            ped.Ped.RelationshipGroup = Game.Player.Character.RelationshipGroup;

            _pedActingScript = InstantiateScript<PedActingScript>();
            _pedActingScript.WPed = ped;

            _destinationBlip = WBlipHelper.GetMission(DestinationMessageKey);
            _destinationBlip.Position = Destination.Position;

        }

        protected override void CleanScene()
        {
            _destinationBlip?.Remove();
        }
        
    }
}