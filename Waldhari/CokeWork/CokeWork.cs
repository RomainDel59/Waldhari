using System;
using GTA;
using Waldhari.CokeWork.Missions;
using Waldhari.Common;
using Waldhari.Common.Behavior.Mission.Helper;
using Waldhari.Common.Behavior.Property;
using Waldhari.Common.Files;
using Waldhari.Common.Misc;
using Waldhari.Common.UI;

namespace Waldhari.CokeWork
{
    public class CokeWork : Script
    {
        private int _nextExecution = Game.GameTime;

        public Property Property;

        private BuyablePropertyScript _scriptToBuy;
        private bool _isWaitingForBuyer;
        
        
        
        

        public CokeWork()
        {
            Load();

            Property = new Property
            {
                NameKey = "cokework",
                Holder = (Property.Owner)CokeWorkSave.Instance.Owner,
                Price = CokeWorkOptions.Instance.Price,
                Position = CokeWorkHelper.Positions.Property.Position,
                Sprite = BlipSprite.Cocaine,
            };
            
            Property.ShowBlip();
            
            Tick += OnTick;
        }
        
        private void OnTick(object sender, EventArgs e)
        {
            CokeWorkHelper.ProcessMenu();
            PhoneHelper.ManageContact(CokeWorkHelper.GetContact(), Property.Holder);
            
            // if not bought and not waiting for buyer, make it wait for a buyer
            if (!Property.IsOwned() && !_isWaitingForBuyer)
            {
                MakeWaitForBuyer();
                return;
            }

            // Nothing to do if not bought
            if (!Property.IsOwned()) return;
            _isWaitingForBuyer = false;
            
            // To lower material usage :
            // runs this script every 1/2 second only
            if (_nextExecution > Game.GameTime) return;
            _nextExecution = Game.GameTime + 500;

            if(!_manufactureStarted)
            {
                CokeWorkHelper.StartManufacture();
                _manufactureStarted = true;
            }
            
            DefenseMissionHelper.TryToStart<CokeWorkDefenseScript>(CokeWorkHelper.Positions.Property, CokeWorkSave.Instance.Product);
            
        }

        private static bool _manufactureStarted;

        private void MakeWaitForBuyer()
        {
            Logger.Info("Not bought and not waiting for buyer, making it wait for a buyer");

            var buyableProperty = new BuyableProperty(Property)
            {
                HelpKey = "cokework_help",
                BuySuccessKey = "cokework_buy_success",
                BuyFailureKey = "cokework_buy_failure"
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
            CokeWorkSave.Instance.Owner = (int)Property.Holder; 
            CokeWorkSave.Instance.Save();
        }

        private void ShowIntroMessage()
        {
            Logger.Info("Showing intro message");
            CokeWorkHelper.ShowFromContact("cokework_intro");
        }

        private void Load()
        {
            OptionsHelper.GlobalLoad();
            new CokeWorkOptions().Load();
            new CokeWorkSave().Load();
        }
        
    }
}