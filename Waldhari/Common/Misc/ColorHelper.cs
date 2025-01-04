using System.Drawing;
using GTA;
using Waldhari.Common.Exceptions;

namespace Waldhari.Common.Misc
{
    public static class ColorHelper
    {
        

        /// <summary>
        /// Returns the system color of the current Blip Color.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="TechnicalException"></exception>
        public static Color GetSystemColor(BlipColor blipColor)
        {
            switch (blipColor)
            {
                case BlipColor.White: return Color.White;
                case BlipColor.Yellow: return Color.Yellow;
                case BlipColor.Blue: return Color.Blue;
                case BlipColor.Green: return Color.Green;
                case BlipColor.Red: return Color.Red;
                case BlipColor.Orange: return Color.Orange;
                case BlipColor.Purple: return Color.Purple;
                case BlipColor.Pink: return Color.Pink;
                case BlipColor.Michael: return Color.SkyBlue;
                case BlipColor.Trevor: return Color.SandyBrown;
                case BlipColor.Franklin: return Color.LightGreen;
                default: throw new TechnicalException($"Blip color {blipColor} is unknown.");
            }
        }
        
        public static BlipColor GetOwnerBlipColor(int ownerId)
        {
            switch (ownerId)
            {
                case 1: return BlipColor.Michael;
                case 2: return BlipColor.Trevor;
                case 3: return BlipColor.Franklin;
                default: return BlipColor.White;
            }
        }
        
        
        
    }
}