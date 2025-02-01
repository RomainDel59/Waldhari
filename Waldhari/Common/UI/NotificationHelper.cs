using System.Collections.Generic;
using GTA.Native;
using GTA.UI;
using Waldhari.Common.Files;

namespace Waldhari.Common.UI
{
    public static class NotificationHelper
    {
        public const string InputContextSecondary = "INPUT_CONTEXT_SECONDARY";

        public static string GetInputContextSecondary()
        {
            return $"~{InputContextSecondary}~";
        }
        
        
        public static void Show(string messageKey, List<string> messageValues = null)
        {
            var message = Localization.GetTextByKey(messageKey, messageValues);

            Notification.Show(message);
        }

        public static void ShowWithIcon(NotificationIcon icon, string senderKey, string messageKey, List<string> messageValues = null)
        {
            var sender = Localization.GetTextByKey(senderKey);
            var message = Localization.GetTextByKey(messageKey, messageValues);

            SoundHelper.PlayEmail();
            Notification.Show(icon, sender, string.Empty, message, true);
        }
        
        public static void ShowFromDefault(string messageKey, string senderKey, List<string> messageValues = null)
        {
            ShowWithIcon(NotificationIcon.Default, senderKey, messageKey, messageValues);
        }
        
        public static void ShowFromSecuroServ(string messageKey, List<string> messageValues = null)
        {
            //todo: how to use CHAR_GANGAPP icon ??
            ShowWithIcon(NotificationIcon.Default, "securoserv", messageKey, messageValues);
        }

        public static void Subtitle(string messageKey, int seconds = 5)
        {
            Screen.ShowSubtitle(
                message: Localization.GetTextByKey(messageKey),
                duration: seconds * 1000
            );
        }

        public static void HideSubtitle()
        {
            Screen.ShowSubtitle(string.Empty, 1);
        }

        public static void ShowSuccess(string messageKey, List<string> messageValues = null)
        {
            SoundHelper.PlaySuccess();
            Show(messageKey, messageValues);
        }

        public static void ShowFailure(string messageKey, List<string> messageValues = null)
        {
            SoundHelper.PlayFailure();
            Show(messageKey, messageValues);
        }

        public static void ShowHelp(string messageKey, List<string> messageValues = null, int duration = 20)
        {
            Screen.ShowHelpText(
                Localization.GetTextByKey(messageKey, messageValues),
                duration > 0 ? duration * 1000 : -1
            );
        }

        public static void HideHelp()
        {
            Function.Call(Hash.CLEAR_HELP, true);
        }
    }
}