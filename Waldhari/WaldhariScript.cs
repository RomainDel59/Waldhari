using System;
using System.Globalization;
using GTA;
using Waldhari.Common.Files;
using Waldhari.Common.UI;

namespace Waldhari
{
    public class WaldhariScript : Script
    {
        public WaldhariScript()
        {
            Tick += OnTick;
        }

        
        private void OnTick(object sender, EventArgs e)
        {
            PhoneHelper.GetIFruit().Update();
            //Logger.Debug($"iFuit num contacts={PhoneHelper.GetIFruit().Contacts.Count}");
            
            if (Game.IsControlJustPressed(Control.ContextSecondary))
            {
                var position = Environment.NewLine+"= new WPosition"+Environment.NewLine +
                               "{"+Environment.NewLine +
                               "Position = new Vector3(" +
                               Game.Player.Character.Position.X.ToString(CultureInfo.CurrentCulture).Replace(",", ".") +
                               "f, " +
                               Game.Player.Character.Position.Y.ToString(CultureInfo.CurrentCulture).Replace(",", ".") +
                               "f, " +
                               Game.Player.Character.Position.Z.ToString(CultureInfo.CurrentCulture).Replace(",", ".") +
                               "f),"+Environment.NewLine +
                               "Rotation = new Vector3(" +
                               Game.Player.Character.Rotation.X.ToString(CultureInfo.CurrentCulture).Replace(",", ".") +
                               "f, " +
                               Game.Player.Character.Rotation.Y.ToString(CultureInfo.CurrentCulture).Replace(",", ".") +
                               "f, " +
                               Game.Player.Character.Rotation.Z.ToString(CultureInfo.CurrentCulture).Replace(",", ".") +
                               "f),"+Environment.NewLine +
                               "Heading = " + Game.Player.Character.Heading.ToString(CultureInfo.CurrentCulture).Replace(",", ".") + "f"+Environment.NewLine +
                               "};";
                Logger.Debug(position);
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                //test
                 // _test = InstantiateScript<EnemyGroupScript>();
                 // _test?.DefineGroup(WGroupHelper.CreateRivalMembers(10));
            }

            if (Game.IsControlJustPressed(Control.Context))
            {
                //test
                // _test?.MarkAsNoLongerNeeded();
                // _test?.Abort();
                // _test = null;
            }
        }
        // private EnemyGroupScript _test;
    }
}