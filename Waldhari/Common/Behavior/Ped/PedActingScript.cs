using System;
using GTA;
using GTA.Native;
using Waldhari.Common.Entities;
using Waldhari.Common.Entities.Helpers;
using Waldhari.Common.Enums;
using Waldhari.Common.Exceptions;

namespace Waldhari.Behavior.Ped
{
    [ScriptAttributes(NoDefaultInstance = true)]
    public class PedActingScript : Script
    {
        private int _nextExecution = Game.GameTime;
        
        public WPed WPed;
        
        private bool _hasMoved;
        
        private bool _isGoingBackPosition;
        
        public bool IsInCombat() => WPed.Ped.IsInCombat;
        
        public bool IsPlayingScenario() => Function.Call<bool>(Hash.IS_PED_ACTIVE_IN_SCENARIO, WPed.Ped.Handle);

        private bool HasScenarioToDo() => !string.IsNullOrEmpty(WPed.Scenario);

        private bool HasAnimationToDo() => !string.IsNullOrEmpty(WPed.AnimationDictionnary) &&
                                           !string.IsNullOrEmpty(WPed.AnimationName);
        
        public PedActingScript()
        {
            Tick += OnTick;
            Aborted += OnAborted;
        }

        private void OnAborted(object sender, EventArgs e)
        {
            if (WPed == null) return;
            
            WPed.Ped?.MarkAsNoLongerNeeded();
            WPed.WBlip?.Remove();
        }

        private void OnTick(object sender, EventArgs e)
        {
            // Wait for parameter
            if(WPed == null) return;
            
            if (!HasAnimationToDo() && !HasScenarioToDo()) 
                throw new TechnicalException("Ped should have a scenario or an animation to play");
            
            // To lower material usage :
            // runs this script every 2 seconds only
            if (_nextExecution > Game.GameTime) return;
            _nextExecution = Game.GameTime + 2000;
            
            // If ped is in combat, let the ped fight
            if (IsInCombat()) return;
            
            // If ped is away from initial position, let it goes back to position
            if (!WPositionHelper.IsNear(WPed.Ped.Position,WPed.InitialPosition.Position,0.5f))
            {
                
                // If ped is already going back, wait it to proceed
                if (_isGoingBackPosition) return;
            
                // Makes ped run to position
                RunTo();
                
                _hasMoved = true;
                _isGoingBackPosition = true;
                
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
            WPed.Ped.Task.PlayAnimation(WPed.AnimationDictionnary, WPed.AnimationName, 8f,-8f,-1,AnimationFlags.Loop,0f);
        }

        private void PlayScenario()
        {
            WPed.Ped.Task.StartScenario(WPed.Scenario, WPed.InitialPosition.Position, WPed.InitialPosition.Heading);
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
                WPed.Ped.Handle, 
                WPed.InitialPosition.Position.X, 
                WPed.InitialPosition.Position.Y,
                WPed.InitialPosition.Position.Z,
                EMovementRatio.Run, 
                timeout, 
                distanceToDestination, 
                ENavigation.Default, 
                WPed.InitialPosition.Heading);
            
        }
        
        
        
    }
}