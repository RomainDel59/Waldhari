using System;
using GTA;
using Waldhari.Common;
using Waldhari.Common.Behavior.Mission.Helper;
using Waldhari.Common.Behavior.Property;
using Waldhari.Common.Entities.Helpers;
using Waldhari.Common.Files;
using Waldhari.Common.Misc;
using Waldhari.Common.UI;
using Waldhari.MethLab.Missions;

namespace Waldhari.MethLab
{
    public class MethLab : Script
    {
        private int _nextExecution = Game.GameTime;

        public Property Property;

        private BuyablePropertyScript _scriptToBuy;
        private bool _isWaitingForBuyer;
        
        
        
        

        public MethLab()
        {
            Load();

            Property = new Property
            {
                NameKey = "methlab",
                Holder = (Property.Owner)MethLabSave.Instance.Owner,
                Price = MethLabOptions.Instance.Price,
                Position = MethLabHelper.Positions.Property.Position,
                Sprite = BlipSprite.Meth
            };
            
            Property.ShowBlip();
            
            Tick += OnTick;
        }
        
        private void OnTick(object sender, EventArgs e)
        {
            MethLabHelper.ProcessMenu();
            PhoneHelper.ManageContact(MethLabHelper.GetContact(), Property.Holder);
            
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
                MethLabHelper.StartManufacture();
                _manufactureStarted = true;
            }
            
            DefenseMissionHelper.TryToStart<MethLabDefenseScript>(MethLabHelper.Positions.Property, MethLabSave.Instance.Product);

            TryOpenDoors();

        }

        private bool _doorsOpened;
        private void TryOpenDoors()
        {
            //var position = new Vector3(1393.0f, 3599.5f, 35.0f);
            var position = MethLabHelper.Positions.Property.Position;

            // If player is within 300 meters near
            if (WPositionHelper.IsNearPlayer(position, 300))
            {
                // If doors already opened
                if (_doorsOpened) return;
                
                // Open doors
                OpenDoors(true);
            }
            // If the player is more than 300 meters away
            else
            {
                // If doors already closed
                if (!_doorsOpened) return;

                // Close doors
                OpenDoors(false);
            }


        }

        private void OpenDoors(bool open)
        {
            const int frontLeft = 212192855;
            const int frontRight = -126474752;
            const int rear = 1765671336;

            DoorHelper.Open(frontLeft, open);
            DoorHelper.Open(frontRight, open);
            DoorHelper.Open(rear, open);
            
            _doorsOpened = open;
        }

        private static bool _manufactureStarted;

        private void MakeWaitForBuyer()
        {
            Logger.Info("Not bought and not waiting for buyer, making it wait for a buyer");

            var buyableProperty = new BuyableProperty(Property)
            {
                HelpKey = "methlab_help",
                BuySuccessKey = "methlab_buy_success",
                BuyFailureKey = "methlab_buy_failure"
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
            MethLabSave.Instance.Owner = (int)Property.Holder; 
            MethLabSave.Instance.Save();
        }

        private void ShowIntroMessage()
        {
            Logger.Info("Showing intro message");
            MethLabHelper.ShowFromContact("methlab_intro");
            
        }

        private void Load()
        {
            OptionsHelper.GlobalLoad();
            new MethLabOptions().Load();
            new MethLabSave().Load();
        }
        
    }
}