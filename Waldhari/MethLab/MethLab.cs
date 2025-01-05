using System;
using GTA;
using GTA.Math;
using Waldhari.Common;
using Waldhari.Common.Entities;
using Waldhari.Common.Entities.Helpers;
using Waldhari.Common.Files;
using Waldhari.Common.Misc;
using Waldhari.Common.Mission;
using Waldhari.Common.Scripts;
using Waldhari.Common.UI;

namespace Waldhari.MethLab
{
    public class MethLab : Script
    {
        private int _nextExecution = Game.GameTime;
        
        public static Vector3 PropertyPosition = new Vector3(1394.762f, 3599.618f, 35.01107f);
        public static WBlip PropertyBlip;

        private PropertyToBuyScript _scriptToBuy = null;
        private bool _isWaitingForBuyer;
        
        private static Vector3 LabPosition = new Vector3(1389.134f, 3605.236f, 38.94193f);

        public MethLab()
        {
            DefineOptions();

            ShowBlip();

            Tick += OnTick;
        }

        private void OnTick(object sender, EventArgs e)
        {
            // if not bought and not waiting for buyer, make it wait for a buyer
            if (!IsOwned() && !_isWaitingForBuyer)
            {
                Logger.Info("Nothing to do if not bought, but");
                
                _scriptToBuy = InstantiateScript<PropertyToBuyScript>();
                var param = new PropertyToBuyScript.Parameters
                {
                    Position = PropertyPosition,
                    NameKey = "methlab",
                    HelpKey = "methlab_buy", 
                    BuySuccessKey = "methlab_buy_success", 
                    BuyFailureKey = "methlab_buy_failure", 
                    Price = MethLabOptions.Instance.Price,
                    IsOwned = IsOwned,
                    ChangeOwner = ChangeOwner,
                    ReloadBlip = ShowBlip,
                    Finish = ShowIntroMessage
                };
                
                _scriptToBuy.Params = param;
                _isWaitingForBuyer = true;
                return;
            }

            // Nothing to do if not bought
            if (!IsOwned()) return;
            
            
            
            // To lower material usage :
            // runs this script every 1/2 second only
            if (_nextExecution > Game.GameTime) return;
            _nextExecution = Game.GameTime + 500;
            
            
        }

        private void ChangeOwner()
        {
            Logger.Info("Changing owner");
            
            MethLabSave.Instance.Owner = PlayerHelper.GetCharacterId();
            MethLabSave.Instance.Save();
        }

        private void ShowIntroMessage()
        {
            Logger.Info("Showing intro message");
            
            NotificationHelper.ShowFromRon("methlab_intro");
        }

        private bool IsNearProperty()
        {
            return WPositionHelper.IsNear(Game.Player.Character.Position, PropertyPosition, 5);
        }

        private bool IsOwned()
        {
            return MethLabSave.Instance.Owner != 0;
        }

        private bool OwnProperty()
        {
            return MethLabSave.Instance.Owner == PlayerHelper.GetCharacterId();
        }

        private void DefineMissionOptions(AbstractMission mission)
        {
            mission.WantedChance = GlobalOptions.Instance.WantedChance;
            
            mission.RivalChance = GlobalOptions.Instance.RivalChance;
            
            mission.RivalMembers = GlobalOptions.Instance.RivalMembers;
            
        }

        private void DefineOptions()
        {
            OptionsHelper.DefineGlobals();

            new MethLabOptions().Load();
            new MethLabSave().Load();
        }

        private void ShowBlip()
        {
            if (!GlobalOptions.Instance.ShowBlips) return;

            PropertyBlip?.Remove();
            PropertyBlip = WBlipHelper.CreateProperty(
                BlipSprite.Meth,
                PropertyPosition,
                MethLabSave.Instance.Owner,
                "methlab",
                MethLabOptions.Instance.Price
            );
        }
        
    }
}