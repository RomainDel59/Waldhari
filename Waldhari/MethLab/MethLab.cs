using System;
using GTA;
using GTA.Math;
using Waldhari.Behavior.Property;
using Waldhari.Common;
using Waldhari.Common.Files;
using Waldhari.Common.Misc;
using Waldhari.Common.Mission;
using Waldhari.Common.UI;

namespace Waldhari.MethLab
{
    public class MethLab : Script
    {
        private int _nextExecution = Game.GameTime;

        public Property Property;

        private BuyablePropertyScript _scriptToBuy;
        private bool _isWaitingForBuyer;
        
        private static Vector3 LabPosition = new Vector3(1389.134f, 3605.236f, 38.94193f);

        public MethLab()
        {
            Load();

            Property = new Property
            {
                NameKey = "methlab",
                Holder = (Property.Owner)MethLabSave.Instance.Owner,
                Price = MethLabOptions.Instance.Price,
                Position = new Vector3(1394.762f, 3599.618f, 35.01107f),
                Sprite = BlipSprite.Meth
            };
            
            Property.ShowBlip();
            
            Tick += OnTick;
        }

        private void OnTick(object sender, EventArgs e)
        {
            // if not bought and not waiting for buyer, make it wait for a buyer
            if (!Property.IsOwned() && !_isWaitingForBuyer)
            {
                Logger.Info("Nothing to do if not bought, but");

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

        private void DefineMissionOptions(AbstractMission mission)
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