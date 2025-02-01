using System;
using GTA;
using Waldhari.Common;
using Waldhari.Common.Behavior.Mission.Helper;
using Waldhari.Common.Behavior.Property;
using Waldhari.Common.Files;
using Waldhari.Common.Misc;
using Waldhari.Common.UI;
using Waldhari.WeedFarm.Missions;

namespace Waldhari.WeedFarm
{
    public class WeedFarm : Script
    {
        private int _nextExecution = Game.GameTime;

        public Property Property;

        private BuyablePropertyScript _scriptToBuy;
        private bool _isWaitingForBuyer;
        
        
        
        

        public WeedFarm()
        {
            Load();

            Property = new Property
            {
                NameKey = "weedfarm",
                Holder = (Property.Owner)WeedFarmSave.Instance.Owner,
                Price = WeedFarmOptions.Instance.Price,
                Position = WeedFarmHelper.Positions.Property.Position,
                Sprite = BlipSprite.Weed,
            };
            
            Property.ShowBlip();
            
            Tick += OnTick;
        }
        
        private static bool _guardScriptStarted;
        
        private void OnTick(object sender, EventArgs e)
        {
            if (!_guardScriptStarted)
            {
                WeedFarmHelper.StartGuardScript();
                _guardScriptStarted = true;
            }
            
            WeedFarmHelper.ProcessMenu();
            PhoneHelper.ManageContact(WeedFarmHelper.GetContact(), Property.Holder);
            
            // if not bought and not waiting for buyer, make it wait for a buyer
            if (!Property.IsOwned() && !_isWaitingForBuyer)
            {
                MakeWaitForBuyer();
                return;
            }

            // Nothing to do if not bought
            if (!Property.IsOwned()) return;
            
            // When property is bought
            _isWaitingForBuyer = false;
            
            // To lower material usage :
            // runs this script every 1/2 second only
            if (_nextExecution > Game.GameTime) return;
            _nextExecution = Game.GameTime + 500;

            if(!_manufactureStarted)
            {
                WeedFarmHelper.StartManufacture();
                _manufactureStarted = true;
            }
            
            DefenseMissionHelper.TryToStart<WeedFarmDefenseScript>(WeedFarmHelper.Positions.Property, WeedFarmSave.Instance.Product);
            
        }

        private static bool _manufactureStarted;

        private void MakeWaitForBuyer()
        {
            Logger.Info("Not bought and not waiting for buyer, making it wait for a buyer");

            var buyableProperty = new BuyableProperty(Property)
            {
                HelpKey = "weedfarm_help",
                BuySuccessKey = "weedfarm_buy_success",
                BuyFailureKey = "weedfarm_buy_failure"
            };

            _scriptToBuy = InstantiateScript<BuyablePropertyScript>();
            _scriptToBuy.ChangeOwner = ChangeOwner;
            _scriptToBuy.DoAtEnd = ShowIntroMessage;
            _scriptToBuy.BuyableProperty = buyableProperty;
            
            _isWaitingForBuyer = true;
        }

        private void ChangeOwner()
        {
            Logger.Info("Changing owner");
            Property.Holder = PlayerHelper.GetCharacterId();
            WeedFarmSave.Instance.Owner = (int)Property.Holder; 
            WeedFarmSave.Instance.Save();
        }

        private void ShowIntroMessage()
        {
            Logger.Info("Showing intro message");
            WeedFarmHelper.ShowFromContact("weedfarm_intro");
        }

        private void Load()
        {
            OptionsHelper.GlobalLoad();
            new WeedFarmOptions().Load();
            new WeedFarmSave().Load();
        }
        
    }
}