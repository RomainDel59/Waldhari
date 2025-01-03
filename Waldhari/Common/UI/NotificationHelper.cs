using System.Collections.Generic;
using GTA.UI;
using Waldhari.Common.Files;

namespace Waldhari.Common.UI
{
    public static class NotificationHelper
    {
        public static void Show(string messageKey, List<string> messageValues = null)
        {
            var message = Localization.GetTextByKey(messageKey, messageValues);

            Notification.Show(message);
        }

        public static void ShowWithIcon(NotificationIcon icon, string senderKey, string messageKey, List<string> messageValues = null)
        {
            var sender = Localization.GetTextByKey(senderKey);
            var message = Localization.GetTextByKey(messageKey, messageValues);

            Notification.Show(icon, sender, string.Empty, message, true);
        }

        public static void ShowFromRon(string messageKey, List<string> messageValues = null)
        {
            ShowWithIcon(NotificationIcon.Ron, "ron_sender", messageKey, messageValues);
        }

        //todo: "chemist_sender"
        public static void ShowFromDefault(string messageKey, string senderKey, List<string> messageValues = null)
        {
            ShowWithIcon(NotificationIcon.Default, senderKey, messageKey, messageValues);
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
    }
}