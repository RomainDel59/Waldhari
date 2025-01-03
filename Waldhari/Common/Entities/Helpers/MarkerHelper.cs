﻿using GTA;
using GTA.Math;

namespace Waldhari.Common.Entities.Helpers
{
    public static class MarkerHelper
    {
        /// <summary>
        /// Draw a marker on the ground in the world accordingly a WBlip position and color.
        /// </summary>
        /// <param name="wBlip">WBlip to use</param>
        public static void DrawGroundMarkerOnBlip(WBlip wBlip)
        {
            var position = wBlip.Position;
            position.Z = World.GetGroundHeight(position);
            
            const float radius = 5.0f;
            const float height = 0.5f;
            World.DrawMarker(
                MarkerType.VerticalCylinder,
                position,
                Vector3.Zero,
                Vector3.Zero,
                new Vector3(radius, radius, height),
                wBlip.GetSystemColor());
        }
        
        /// <summary>
        /// Draw a marker over an entity in the world accordingly a WBlip position and color.
        /// </summary>
        /// <param name="wBlip">WBlip to use</param>
        public static void DrawEntityMarkerOnBlip(WBlip wBlip)
        {
            var position = wBlip.Position;
            position.Z += 2.5f;
            
            const float radius = 0.5f;
            const float height = 0.5f;
            World.DrawMarker(
                MarkerType.UpsideDownCone,
                position,
                Vector3.Zero,
                Vector3.Zero,
                new Vector3(radius, radius, height),
                wBlip.GetSystemColor());
        }
        
    }
}