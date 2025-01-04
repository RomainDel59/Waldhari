using System;
using System.Collections.Generic;
using GTA;
using GTA.Math;
using Waldhari.Common.Entities.Helpers;
using Waldhari.Common.Files;
using Waldhari.Common.Misc;
using Waldhari.Common.UI;

namespace Waldhari.Common.Scripts
{
    [ScriptAttributes(NoDefaultInstance = true)]
    public class PropertyToBuyScript : Script
    {
        public class Structure
        {
            public Vector3 PropertyPosition;
            public string NameKey;
            public string HelpKey;
            public string BuySuccessKey;
            public string BuyFailureKey;
            public int Price;
            public Func<bool> IsOwned;
            public Action ChangeOwner;
            public Action ReloadBlip;
            public Action Finish;
        }
        
        private int _nextExecution = Game.GameTime;
        
        // Parameters required
        public Structure Parameters = null;
        
        // Inner properties
        private bool _helpMessageIsShowing;
        private int _timeToShowSuccess = -1;
        private int _timeToFinish = -1;

        public PropertyToBuyScript()
        {
            Tick += OnTick;
        }

        private void OnTick(object sender, EventArgs e)
        {
            // Wait for parameters
            if (Parameters == null) return;
            
            MissionAnimationHelper.Show();
            
            Buy();
            
            // To lower material usage :
            // runs this script every 1/2 second only
            if (_nextExecution > Game.GameTime) return;
            _nextExecution = Game.GameTime + 500;
            
            ShowHelpMessage();
            
            ShowSuccessMessage();

            Finish();
        }

        private void Buy()
        {
            // Can't buy if message is not showing
            if (!_helpMessageIsShowing) return;
            
            // Can't buy if not press correct control
            if (!Game.IsControlJustPressed(Control.ContextSecondary)) return;
            
            // Can't buy if player is poor
            if (Game.Player.Money < Parameters.Price)
            {
                NotificationHelper.ShowFailure(Parameters.BuyFailureKey);
                return;
            }
            
            NotificationHelper.HideHelp();
            
            SoundHelper.PlayPayment();
            Game.Player.Money -= Parameters.Price;
            Game.DoAutoSave();

            Parameters.ChangeOwner();
            Parameters.ReloadBlip();
            
            MissionAnimationHelper.PropertyBought(Parameters.NameKey);

            _timeToShowSuccess = Game.GameTime + 5*1000;
        }
        
        private void ShowHelpMessage()
        {
            // If quit place and shown already : hide
            if (!IsNearProperty() && _helpMessageIsShowing)
            {
                NotificationHelper.HideHelp();
                _helpMessageIsShowing = false;
                return;
            }
            
            // If property already owned
            if (Parameters.IsOwned.Invoke()) return;
            
            // If player is not near
            if (!IsNearProperty()) return;
            
            // If shown already
            if(_helpMessageIsShowing) return;

            NotificationHelper.ShowHelp(
                Parameters.HelpKey, new List<string>
                {
                    Localization.GetTextByKey(Parameters.NameKey),
                    NumberHelper.ConvertToAmount(Parameters.Price),
                    NotificationHelper.GetInputContextSecondary()
                }
            );
            
            _helpMessageIsShowing = true;
        }

        private void ShowSuccessMessage()
        {
            // If it's not the time to show it yet
            if (_timeToShowSuccess == -1 || _timeToShowSuccess > Game.GameTime) return; 
            
            NotificationHelper.ShowHelp(Parameters.BuySuccessKey);
            
            _timeToFinish = Game.GameTime + 15*1000;
            _timeToShowSuccess = -1;
        }

        private void Finish()
        {
            // If it's not the time to finish yet
            if(_timeToFinish == -1 || _timeToFinish > Game.GameTime) return;
            
            NotificationHelper.HideHelp();
            
            Parameters.Finish();
            _timeToFinish = -1;
            
            Abort();
        }

        private bool IsNearProperty()
        {
            return WPositionHelper.IsNear(Game.Player.Character.Position, Parameters.PropertyPosition, 5);
        }
    }
}