using System.Collections.Generic;
using System.Drawing;
using GTA;
using GTA.Math;
using Waldhari.Common.Exceptions;
using Waldhari.Common.Files;

namespace Waldhari.Common.Entities
{
    public class WBlip
    {
        public Blip Blip = null;
        
        public string NameKey = null;
        public List<string> Values = null;
        public BlipColor BColor = default;
        public BlipSprite Sprite = default;
        public bool IsShortRange = false;
        public bool IsVisible = false;
        public bool IsShowingRoute = false;
        
        // Only one of these three entities has to be set, not the three!
        public Vehicle Vehicle = null;
        public Ped Ped = null;
        public Vector3 Position = default;

        /// <summary>
        /// Creates the blip according entered properties.
        /// If the blip has already been created, does nothing.
        /// Developer has to define : Vehicle, Ped or Position
        /// to define where the Blip is created.
        /// </summary>
        public void Create()
        {
            if (Blip != null) return;
            
            if(Vehicle != null)
                CreateOnVehicle();
            else if(Ped != null)
                CreateOnPed();
            else
                CreateOnMap();
        }
        
        /// <summary>
        /// Removes the blip. It deletes it, not only hide. 
        /// If the blip has already been removed, does nothing.
        /// Properties of WBlip class are preserved,
        /// only the blip is delete and its property set to null.
        /// </summary>
        public void Remove()
        {
            Blip?.Delete();
            Blip = null;
        }

        /// <summary>
        /// Show the blip on map and minimap.
        /// </summary>
        public void Show()
        {
            if(Blip == null) throw new TechnicalException("Blip cannot be empty.");
            
            IsVisible = true;
            Blip.DisplayType = BlipDisplayType.BothMapSelectable;
        }

        /// <summary>
        /// Hide the blip on map and minimap.
        /// </summary>
        public void Hide()
        {
            if(Blip == null) throw new TechnicalException("Blip cannot be empty.");
            
            IsVisible = false;
            Blip.DisplayType = BlipDisplayType.NoDisplay;
        }

        /// <summary>
        /// Returns the system color of the current Blip Color.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="TechnicalException"></exception>
        public Color GetSystemColor()
        {
            switch (Blip.Color)
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
                default: throw new TechnicalException($"Blip color {BColor} is unknown.");
            }
        }

        private void DefineProperties()
        {
            if(Blip == null) throw new TechnicalException("Blip cannot be empty.");
            
            //Sprite first, so name and color can be overridden
            if(Sprite != default)
                Blip.Sprite = Sprite;
            
            if(NameKey != null)
                Blip.Name = Localization.GetTextByKey(NameKey,Values);
            
            if(BColor != default)
                Blip.Color = BColor;
            
            Blip.IsShortRange = IsShortRange;
            
            Blip.ShowRoute = IsShowingRoute;
            
            Blip.DisplayType = IsVisible ? BlipDisplayType.Default : BlipDisplayType.NoDisplay;
        }

        private void CreateOnMap()
        {
            if(Position == Vector3.Zero) throw new TechnicalException("Blip position cannot be empty.");
            
            Blip = World.CreateBlip(Position);
            
            DefineProperties();
        }

        private void CreateOnPed()
        {
            if (Ped == null) throw new TechnicalException("Blip ped cannot be empty.");

            Blip = Ped.AddBlip();
            
            DefineProperties();
        }

        private void CreateOnVehicle()
        {
            if (Vehicle == null) throw new TechnicalException("Blip vehicle cannot be empty.");
            
            Blip = Vehicle.AddBlip();
            
            DefineProperties();
        }
        
    }
}