using System.Collections.Generic;
using GTA;
using Waldhari.Common.Files;
using Waldhari.Common.Misc;
using Waldhari.Common.UI;

namespace Waldhari.Common.Behavior.Property
{
    public class BuyableProperty
    {
        public Property Property;
        
        public string HelpKey;
        public string BuySuccessKey;
        public string BuyFailureKey;

        public BuyableProperty(Property property)
        {
            Property = property;
        }

        public void ShowHelp()
        {
            NotificationHelper.ShowHelp(
                HelpKey, 
                new List<string>
                {
                    Localization.GetTextByKey(Property.NameKey),
                    NumberHelper.ConvertToAmount(Property.Price),
                    NotificationHelper.GetInputContextSecondary()
                },
                -1
            );
        }

        public void ShowBuySuccess()
        {
            NotificationHelper.ShowHelp(BuySuccessKey);
        }

        public void ShowBuyFailure()
        {
            NotificationHelper.ShowFailure(BuyFailureKey);
        }

        public void Buy()
        {
            Game.Player.Money -= Property.Price;
            Game.DoAutoSave();
            
            
        }
        
        
        
    }
}