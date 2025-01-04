using System;
using GTA;
using GTA.Math;
using Waldhari.Common;
using Waldhari.Common.Entities;
using Waldhari.Common.Entities.Helpers;

namespace Waldhari.MethLab
{
    public class MethLab : Script
    {
        private int _nextExecution = Game.GameTime;
        
        public static Vector3 PropertyPosition = new Vector3(1394.762f, 3599.618f, 35.01107f);
        public static WBlip PropertyBlip;
        
        private static Vector3 LabPosition = new Vector3(1389.134f, 3605.236f, 38.94193f);



        public MethLab()
        {
            DefineOptions();

            ShowBlip();

            Tick += OnTick;
        }

        private void OnTick(object sender, EventArgs e)
        {
            // To lower material usage :
            // runs this script every 1/2 second only
            if (_nextExecution > Game.GameTime) return;
            _nextExecution = Game.GameTime + 500;
            
            
            
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