using System;
using GTA;
using Waldhari.Common.Exceptions;

namespace Waldhari.Common.Behavior.Animation
{
    [ScriptAttributes(NoDefaultInstance = true)]
    public class AnimationScript : Script
    {
        public Animation Animation;
        
        private int _currentStep = -1;
        private int _timeToNextStep = -1;

        public AnimationScript()
        {
            Tick += OnTick;
        }

        private void OnTick(object sender, EventArgs e)
        {
            // Wait for parameter
            if (Animation == null) return;

            // Begin by the first step
            if(_currentStep == -1) _currentStep = 1;

            switch (_currentStep)
            {
                case 1: 
                    Animation.Create();
                    _timeToNextStep = Game.GameTime;
                    _currentStep = 2;
                    break;
                case 2:
                    if (!Animation.Launch()) break;
                    _timeToNextStep = Game.GameTime + Animation.ShowDuration * 1000;
                    _currentStep = 3;
                    break;
                case 3:
                    if (_timeToNextStep > Game.GameTime)
                    {
                        Animation.Show();
                        break;
                    }
                    _currentStep = 4;
                    break;
                case 4:
                    Animation.FadeOut();
                    _timeToNextStep = Game.GameTime + Animation.FadeOutDuration * 1000;
                    _currentStep = 5;
                    break;
                case 5:
                    if (_timeToNextStep > Game.GameTime)
                    {
                        Animation.Show();
                        break;
                    }
                    _currentStep = 6;
                    break;
                case 6:
                    Animation.Hide();
                    break;
                default:
                    throw new TechnicalException($"Unknown Animation Step {_currentStep}");
            }
            
            if(_currentStep == 6)
            {
                Animation = null;
                _currentStep = -1;
                Abort();
            }
        }
    }
}