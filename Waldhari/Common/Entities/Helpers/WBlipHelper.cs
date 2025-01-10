using System.Collections.Generic;
using GTA;
using GTA.Math;
using GTA.Native;
using Waldhari.Behavior.Property;
using Waldhari.Common.Files;
using Waldhari.Common.Misc;

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
                IsVisible = true,
                BColor = BlipColor.Red
            };
            
            return wBlip;
        }

        public static WBlip CreateProperty(BlipSprite sprite, Vector3 position, Property.Owner owner, string nameKey, int price)
        {
            var values = new List<string>();
            if (owner == Property.Owner.None)
            {
                var name = Localization.GetTextByKey(nameKey);
                nameKey = "property_with_price";
                
                values.Add(name);
                values.Add(NumberHelper.ConvertToAmount(price));
                
                Logger.Debug("name="+name);
                Logger.Debug("price="+price);
                Logger.Debug("NumberHelper.ConvertToAmount(price)="+NumberHelper.ConvertToAmount(price));
            }
            
            var propertyBlip = new WBlip
            {
                NameKey = nameKey,
                Values = values,
                Position = position,
                Sprite = sprite,
                BColor = ColorHelper.GetOwnerBlipColor(owner),
                IsShortRange = true,
                IsVisible = true
            };

            propertyBlip.Create();
            
            propertyBlip.Blip.CategoryType = BlipCategoryType.Property;

            // https://docs.fivem.net/natives/?_0xE2590BC29220CEBB
            Function.Call(Hash.SET_BLIP_HIGH_DETAIL, propertyBlip.Blip, false);
            
            //SHOW_FOR_SALE_ICON_ON_BLIP
            Function.Call((Hash)0x19BD6E3C0E16A8FA, propertyBlip.Blip, owner == 0);
            

            return propertyBlip;
        }
    }
}