using System.Collections.Generic;
using Waldhari.Common.UI;

namespace Waldhari.MethLab.Helpers
{
    public static class MethLabNotificationHelper
    {
        public static void ShowFromChemist(string messageKey, List<string> messageValues = null)
        {
            NotificationHelper.ShowFromDefault(messageKey, "chemist_sender", messageValues);
        }
    }
}