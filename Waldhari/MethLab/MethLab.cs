using System;
using GTA;
using iFruitAddon2;
using Waldhari.Behavior.Mission;
using Waldhari.Behavior.Property;
using Waldhari.Common;
using Waldhari.Common.Files;
using Waldhari.Common.Misc;
using Waldhari.Common.UI;

namespace Waldhari.MethLab
{
    public class MethLab : Script
    {
        private int _nextExecution = Game.GameTime;

        public Property Property;

        private BuyablePropertyScript _scriptToBuy;
        private bool _isWaitingForBuyer;
        
        private readonly CustomiFruit _iFruit;
        
        
        

        public MethLab()
        {
            Load();

            Property = new Property
            {
                NameKey = "methlab",
                Holder = (Property.Owner)MethLabSave.Instance.Owner,
                Price = MethLabOptions.Instance.Price,
                Position = MethLabHelper.PropertyPosition,
                Sprite = BlipSprite.Meth
            };
            
            Property.ShowBlip();
            
            _iFruit = new CustomiFruit();
            var contactName = Localization.GetTextByKey("ron_sender") + " (" + Localization.GetTextByKey("methlab") + ")";
            var contact = new iFruitContact(contactName)
            {
                DialTimeout = 2000,            // Delay before answering
                Active = true,                 // true = the contact is available and will answer the phone
                Icon = ContactIcon.Ron       // Contact's icon
            };
            contact.Answered += ShowMenu;   // Linking the Answered event with our function
            _iFruit.Contacts.Add(contact);         // Add the contact to the phone
            
            Tick += OnTick;
        }

        private void ShowMenu(iFruitContact contact)
        {
            //todo: show menu
            _iFruit.Close(1000);
        }

        private void OnTick(object sender, EventArgs e)
        {
            _iFruit.Update();
            
            // if not bought and not waiting for buyer, make it wait for a buyer
            if (!Property.IsOwned() && !_isWaitingForBuyer)
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
                return;
            }

            // Nothing to do if not bought
            if (!Property.IsOwned()) return;
            _isWaitingForBuyer = false;
            
            // To lower material usage :
            // runs this script every 1/2 second only
            if (_nextExecution > Game.GameTime) return;
            _nextExecution = Game.GameTime + 500;
            
            
            
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
            NotificationHelper.ShowFromRon("methlab_intro");
        }

        private void DefineMissionOptions(AbstractMissionScript mission)
        {
            mission.WantedChance = GlobalOptions.Instance.WantedChance;
            mission.RivalChance = GlobalOptions.Instance.RivalChance;
            mission.RivalMembers = GlobalOptions.Instance.RivalMembers;
        }

        private void Load()
        {
            OptionsHelper.GlobalLoad();
            new MethLabOptions().Load();
            new MethLabSave().Load();
        }
        
    }
}