using GTA;

namespace Waldhari.Common.Entities.Helpers
{
    public static class WBlipHelper
    {
        /// <summary>
        /// Returns a mission blip to create.
        /// </summary>
        /// <param name="nameKey">Name of the blip</param>
        public static WBlip GetMission(string nameKey)
        {
            var wBlip = new WBlip
            {
                NameKey = nameKey,
                BColor = BlipColor.Yellow,
                IsShowingRoute = true,
                IsVisible = true
            };
            
            return wBlip;
        }
        
        /// <summary>
        /// Returns an enemy blip to create.
        /// </summary>
        /// <param name="nameKey">Name of the blip</param>
        public static WBlip GetEnemy(string nameKey)
        {
            var wBlip = new WBlip
            {
                NameKey = nameKey,
                Sprite = BlipSprite.Enemy,
                IsShowingRoute = true,
                IsVisible = true
            };
            
            return wBlip;
        }
        
    }
}