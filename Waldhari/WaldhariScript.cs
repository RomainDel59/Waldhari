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
            //todo: add notification icons ?
            //AddSecuroServIcon();
            
            Tick += OnTick;
        }

        
        private void OnTick(object sender, EventArgs e)
        {
            PhoneHelper.GetCharacterPhone().Update();
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
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
                
            }

            if (Game.IsControlJustPressed(Control.Context))
            {
                //test
                // var script = InstantiateScript<GenericGuardMissionScript>();
                // script.Start();
            }
        }
        
        //todo: add notification icons ?
        //
        // private void AddSecuroServIcon()
        // {
        //     string newIcon = "CHAR_GANGAPP";
        //
        //     // Retrieve the s_iconNames field using reflection
        //     FieldInfo field = typeof(Notification).GetField("s_iconNames", BindingFlags.NonPublic | BindingFlags.Static);
        //
        //     if (field == null)
        //     {
        //         GTA.UI.Screen.ShowSubtitle("Error: Unable to find s_iconNames.");
        //         return;
        //     }
        //
        //     // Get the existing array
        //     string[] oldArray = (string[])field.GetValue(null);
        //
        //     // Create a new array with an increased size
        //     string[] newArray = new string[oldArray.Length + 1];
        //
        //     // Copy the old array into the new one
        //     oldArray.CopyTo(newArray, 0);
        //
        //     // Add the new icon at the last position
        //     newArray[newArray.Length - 1] = newIcon;
        //
        //     // Replace the array in the Notification class
        //     field.SetValue(null, newArray);
        // }

    }
}