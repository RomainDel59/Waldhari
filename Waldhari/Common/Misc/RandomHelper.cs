using System;
using Waldhari.Common.Files;

namespace Waldhari.Common.Misc
{
    public static class RandomHelper
    {
        /// <summary>
        /// Uses to create a random value.
        /// </summary>
        private static readonly Random Random = new Random();

        /// <summary>
        /// Returns a random value between two values.
        /// </summary>
        /// <param name="minValue">Min value.</param>
        /// <param name="maxValue">Max value.</param>
        /// <returns></returns>
        public static int Next(int minValue, int maxValue)
        {
            return Random.Next(minValue, maxValue);
        }

        /// <summary>
        /// Returns true if chance is over a random value between 0 and 100.
        /// </summary>
        /// <param name="chance">Chance to win the try.</param>
        /// <returns>Chance is over a random value between 0 and 100</returns>
        public static bool Try(int chance)
        {
            var trying = Next(0, 100 + 1);
            Logger.Debug($"Trying chance={chance}, trying={trying}");
            
            // If chance is over a random percentage
            return chance > trying;
        }
    }
}