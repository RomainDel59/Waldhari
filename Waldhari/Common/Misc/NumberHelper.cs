using System.Globalization;

namespace Waldhari.Common.Misc
{
    public static class NumberHelper
    {
        public static string ConvertToAmount(int number)
        {
            return number.ToString("N0", CultureInfo.CurrentCulture);
        }
    }
}