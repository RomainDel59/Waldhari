using System;
using System.Globalization;
using GTA;
using Waldhari.Common.Files;

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
            if (Game.IsControlJustPressed(Control.ContextSecondary))
            {
                var position = Environment.NewLine+"= new WPosition{"+Environment.NewLine +
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
                Logger.Info(position);
            }
        }
    }
}