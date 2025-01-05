using System;
using GTA;
using Waldhari.Behavior.Animation;
using Waldhari.Common.UI;
using Control = GTA.Control;

namespace Waldhari.Behavior.Property
{
    [ScriptAttributes(NoDefaultInstance = true)]
    public class BuyablePropertyScript : Script
    {
        private int _nextExecution = Game.GameTime;

        public BuyableProperty BuyableProperty;
        public Action ChangeOwner;
        public Action DoAtEnd;

        private bool _isHelpShowing;
        private int _timeToShowSuccess = -1;
        private int _timeToFinish = -1;

        public BuyablePropertyScript()
        {
            Tick += OnTick;
        }

        private void OnTick(object sender, EventArgs e)
        {
            // Wait for parameter
            if (BuyableProperty == null) return;

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
            if (!_isHelpShowing) return;

            // Can't buy if not press correct control
            if (!Game.IsControlJustPressed(Control.ContextSecondary)) return;

            // Can't buy if player is poor
            if (!BuyableProperty.Property.IsPlayerCanBuy())
            {
                BuyableProperty.ShowBuyFailure();
                return;
            }

            NotificationHelper.HideHelp();
            _isHelpShowing = false;

            SoundHelper.PlayPayment();
            Game.Player.Money -= BuyableProperty.Property.Price;
            Game.DoAutoSave();

            ChangeOwner?.Invoke();

            BuyableProperty.Property.ShowBlip();

            var duration = AnimationHelper.PropertyBought(BuyableProperty.Property.NameKey);

            // Wait for animation to end
            _timeToShowSuccess = Game.GameTime + (duration) * 1000;
        }

        private void ShowHelpMessage()
        {
            // If quit place and shown already : hide
            if (!BuyableProperty.Property.IsPlayerNear() && _isHelpShowing)
            {
                NotificationHelper.HideHelp();
                _isHelpShowing = false;
                return;
            }

            // If property already owned
            if (BuyableProperty.Property.IsOwned()) return;

            // If player is not near
            if (!BuyableProperty.Property.IsPlayerNear()) return;

            // If shown already
            if (_isHelpShowing) return;

            BuyableProperty.ShowHelp();
            _isHelpShowing = true;
        }

        private void ShowSuccessMessage()
        {
            // If it's not the time to show it yet
            if (_timeToShowSuccess == -1 || _timeToShowSuccess > Game.GameTime) return;

            BuyableProperty.ShowBuySuccess();

            _timeToFinish = Game.GameTime + 10 * 1000;
            _timeToShowSuccess = -1;
        }

        private void Finish()
        {
            // If it's not the time to finish yet
            if (_timeToFinish == -1 || _timeToFinish > Game.GameTime) return;

            NotificationHelper.HideHelp();

            DoAtEnd?.Invoke();
            _timeToFinish = -1;

            Abort();
        }
    }
}