using System;
using System.Collections.Generic;
using GTA;
using GTA.Math;
using Waldhari.Common;
using Waldhari.Common.Entities;
using Waldhari.Common.Entities.Helpers;
using Waldhari.Common.Files;
using Waldhari.Common.Misc;
using Waldhari.Common.Mission;
using Waldhari.Common.UI;

namespace Waldhari.MethLab
{
    public class MethLab : Script
    {
        private int _nextExecution = Game.GameTime;
        
        public static Vector3 PropertyPosition = new Vector3(1394.762f, 3599.618f, 35.01107f);
        public static WBlip PropertyBlip;
        
        private static Vector3 LabPosition = new Vector3(1389.134f, 3605.236f, 38.94193f);

        private int _nextBuyHelpMessage = Game.GameTime;
        private int _nextHelpMessage = -1;
        
        private int _nextRonMessage = -1;
        private string _ronMessage = string.Empty;


        public MethLab()
        {
            DefineOptions();

            ShowBlip();

            Tick += OnTick;
        }

        private void OnTick(object sender, EventArgs e)
        {
            MissionAnimationHelper.Show();
            
            Buy();

            // To lower material usage :
            // runs this script every 1/2 second only
            if (_nextExecution > Game.GameTime) return;
            _nextExecution = Game.GameTime + 500;

            ShowHelpBuy();

            ShowHelpMessage();

            ShowRonMessage();


        }

        private void ShowHelpMessage()
        {
            if(_nextHelpMessage == -1 || _nextHelpMessage > Game.GameTime) return;
            
            NotificationHelper.ShowHelp("methlab_buy_success");
            _nextHelpMessage = -1;

        }

        private void ShowRonMessage()
        {
            if(_nextRonMessage == -1 || _nextRonMessage > Game.GameTime) return;
            
            NotificationHelper.ShowFromRon(_ronMessage);
            _nextRonMessage = -1;
        }

        private void Buy()
        {
            // If property isn't owned and player want to buy it
            if (IsOwned() || !IsNearProperty() || !Game.IsControlJustPressed(Control.ContextSecondary)) return;

            if (Game.Player.Money < MethLabOptions.Instance.Price)
            {
                NotificationHelper.ShowFailure("methlab_buy_failure");
                return;
            }

            NotificationHelper.HideHelp();
            
            SoundHelper.PlayPayment();
            Game.Player.Money -= MethLabOptions.Instance.Price;
            Game.DoAutoSave();
            
            MethLabSave.Instance.Owner = PlayerHelper.GetCharacterId();
            MethLabSave.Instance.Save();
            
            ShowBlip();
            
            MissionAnimationHelper.PropertyBought("methlab");
            
            _nextHelpMessage = Game.GameTime + 10*1000;
            
            _nextRonMessage = Game.GameTime + 25*1000;
            _ronMessage = "methlab_ron_intro";
        }

        private void ShowHelpBuy()
        {
            // If property isn't owned and player is near
            if (IsOwned() || !IsNearProperty()) return;
            
            // Only every 5 minutes
            if(_nextBuyHelpMessage > Game.GameTime) return;
            _nextBuyHelpMessage = Game.GameTime + 5*60*1000;

            NotificationHelper.ShowHelp(
                "methlab_buy", new List<string>
                {
                    Localization.GetTextByKey("methlab"),
                    NumberHelper.ConvertToAmount(MethLabOptions.Instance.Price),
                    NotificationHelper.GetInputContextSecondary()
                }
            );
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