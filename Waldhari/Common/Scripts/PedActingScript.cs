using System;
using GTA;
using GTA.Native;
using Waldhari.Common.Entities;
using Waldhari.Common.Enums;
using Waldhari.Common.Exceptions;

namespace Waldhari.Common.Scripts
{
    public class PedActingScript : Script
    {
        private WPed _wPed;
        
        private bool _hasMoved;
        
        private bool _isGoingBackPosition;
        
        private bool IsInCombat() => _wPed.Ped.IsInCombat;
        
        private bool IsPlayingScenario() => Function.Call<bool>(Hash.IS_PED_ACTIVE_IN_SCENARIO, _wPed.Ped.Handle);
        private bool _isPlayingAnimation;

        private bool HasScenarioToDo() => !string.IsNullOrEmpty(_wPed.Scenario);

        private bool HasAnimationToDo() => !string.IsNullOrEmpty(_wPed.AnimationDictionnary) &&
                                           !string.IsNullOrEmpty(_wPed.AnimationName);

        private int _nextExecution;
        
        public PedActingScript(WPed wPed)
        {
            if (!HasAnimationToDo() && !HasScenarioToDo())
                throw new TechnicalException("Ped should have at least scenario or animation to play");
            
            _wPed = wPed;

            Tick += OnTick;

            _nextExecution = Game.GameTime;
        }

        private void OnTick(object sender, EventArgs e)
        {
            // To lower material usage :
            // runs this script every 2 seconds only
            if (_nextExecution > Game.GameTime) return;
            _nextExecution = Game.GameTime + 2000;
            
            // If ped is in combat, let the ped fight
            if (IsInCombat()) return;
            
            // If ped is away from initial position, let it goes back to position
            if (_wPed.Ped.Position.DistanceTo(_wPed.InitialPosition.Position) > 0f)
            {
                
                // If ped is already going back, wait it to proceed
                if (_isGoingBackPosition) return;
            
                // Makes ped run to position
                RunTo();
                
                _hasMoved = true;
                _isGoingBackPosition = true;
                _isPlayingAnimation = false;
                
                return;
            }
            
            // Ped is at destination
            _isGoingBackPosition = false;

            // Ped hasn't moved : let's check if scenario has to be replayed
            if (!_hasMoved)
            {
                // Animation is looping with native function
                if (HasAnimationToDo()) return;

                // Scenario is playing
                if (IsPlayingScenario()) return;

                // Replay scenario
                PlayScenario();

            }

            // If ped has to play a scenario
            if (HasScenarioToDo())
            {
                // Play scenario
                PlayScenario();
                return;
            }
            
            // Otherwise play animation
            PlayAnimation();
            
        }

        private void PlayAnimation()
        {
            _wPed.Ped.Task.PlayAnimation(_wPed.AnimationDictionnary, _wPed.AnimationName, 8f,-8f,-1,AnimationFlags.Loop,0f);
        }

        private void PlayScenario()
        {
            _wPed.Ped.Task.StartScenario(_wPed.Scenario, _wPed.InitialPosition.Position, _wPed.InitialPosition.Heading);
        }

        private void RunTo()
        {
            // See https://docs.fivem.net/natives/?_0x15D3A79D4E44B913 for documentation
            
            // No timeout
            const float timeout = -1f;
            
            // No difference
            const float distanceToDestination = 0.0f;
            
            Function.Call(
                Hash.TASK_FOLLOW_NAV_MESH_TO_COORD, 
                _wPed.Ped.Handle, 
                _wPed.InitialPosition.Position.X, 
                _wPed.InitialPosition.Position.Y,
                _wPed.InitialPosition.Position.Z,
                EMovementRatio.Run, 
                timeout, 
                distanceToDestination, 
                ENavigation.Default, 
                _wPed.InitialPosition.Heading);
            
        }
        
    }
}