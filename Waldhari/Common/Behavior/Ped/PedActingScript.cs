using System;
using GTA;
using GTA.Native;
using Waldhari.Common.Entities;
using Waldhari.Common.Entities.Helpers;
using Waldhari.Common.Exceptions;
using Waldhari.Common.Files;

namespace Waldhari.Common.Behavior.Ped
{
    [ScriptAttributes(NoDefaultInstance = true)]
    public class PedActingScript : Script
    {
        public WPed WPed;
        
        private bool _hasMoved;
        
        private bool _isGoingBackPosition;
        private bool _actingStopped;
        
        public bool IsInCombat() => WPed.Ped.IsInCombat;
        
        public bool IsPlayingScenario() => Function.Call<bool>(Hash.IS_PED_ACTIVE_IN_SCENARIO, WPed.Ped.Handle);

        private bool HasScenarioToDo() => !string.IsNullOrEmpty(WPed.Scenario);

        private bool HasAnimationToDo() => !string.IsNullOrEmpty(WPed.AnimationDictionnary) &&
                                           !string.IsNullOrEmpty(WPed.AnimationName);
        
        public PedActingScript()
        {
            Tick += OnTick;
            Aborted += OnAborted;
            _hasMoved = true; //make it play animation or scenario the first time
        }

        private void OnAborted(object sender, EventArgs e)
        {
            WPed?.Ped?.MarkAsNoLongerNeeded();
            WPed?.WBlip?.Remove();
        }

        private void OnTick(object sender, EventArgs e)
        {
            // Wait for parameter
            if(WPed == null) return;
            
            if(_actingStopped) return;

            if (!HasAnimationToDo() && !HasScenarioToDo())
                throw new TechnicalException("Ped should have a scenario or an animation to play");
            
            // If ped is in combat, let the ped fight
            if (IsInCombat()) return;
            
            if (!WPositionHelper.IsNear(WPed.Ped.Position,WPed.InitialPosition.Position,0.5f))
            {
                if (_isGoingBackPosition)
                {
                    //Logger.Debug($"Ped is going back to initial position (scenario={WPed.Scenario}, animationName={WPed.AnimationName})");
                    return;
                }

                Logger.Debug($"Ped moved from initial position (scenario={WPed.Scenario}, animationName={WPed.AnimationName}) : make it run back");
                _hasMoved = true;
                WPed.Ped.Task.RunTo(WPed.InitialPosition.Position);
                _isGoingBackPosition = true;
            }
            else
            {
                _isGoingBackPosition = false;

                if (_hasMoved)
                {
                    Logger.Debug($"Ped is at initial position (scenario={WPed.Scenario}, animationName={WPed.AnimationName}) : make it do acting");
                    WPed.MoveInPosition();
                    if (HasScenarioToDo()) PlayScenario();
                    else if (HasAnimationToDo()) PlayAnimation();
                    _hasMoved = false;
                }
                else
                {
                    // TODO: (immersive feature) If scenario ends, should do it again
                }
            }
            
        }

        private void PlayAnimation()
        {
            Logger.Debug($"Playing animation={WPed.AnimationName}");
            WPed.Ped.Task.GuardCurrentPosition();
            WPed.Ped.Task.PlayAnimation(WPed.AnimationDictionnary, WPed.AnimationName, 8f,-8f,-1,AnimationFlags.Loop,0f);
        }

        private void PlayScenario()
        {
            Logger.Debug($"Playing scenario={WPed.Scenario}");
            WPed.Ped.Task.GuardCurrentPosition();
            WPed.Ped.Task.StartScenario(WPed.Scenario, WPed.InitialPosition.Heading);
        }

        public void StopActing()
        {
            WPed?.Ped?.Task?.ClearAll();
            _actingStopped = true;
        }

        public void RestartActing()
        {
            _actingStopped = false;
        }
    }
}