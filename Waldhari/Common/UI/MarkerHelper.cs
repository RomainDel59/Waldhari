using GTA;
using GTA.Math;
using Waldhari.Common.Entities;
using Waldhari.Common.Misc;

namespace Waldhari.Common.UI
{
    public static class MarkerHelper
    {
        /// <summary>
        /// Draw a marker on the ground in the world accordingly a WBlip position and color.
        /// </summary>
        /// <param name="wBlip">WBlip to use</param>
        public static void DrawGroundMarkerOnBlip(WBlip wBlip, float radius = 5.0f)
        {
            if(wBlip == null) return;
            
            var position = wBlip.Position;
            position.Z = World.GetGroundHeight(position);
            
            const float height = 0.5f;
            World.DrawMarker(
                MarkerType.VerticalCylinder,
                position,
                Vector3.Zero,
                Vector3.Zero,
                new Vector3(radius, radius, height),
                ColorHelper.GetSystemColor(wBlip.BColor));
        }
        
        /// <summary>
        /// Draw a marker over an entity in the world accordingly a WBlip position and color.
        /// </summary>
        /// <param name="wBlip">WBlip to use</param>
        public static void DrawEntityMarkerOnBlip(WBlip wBlip)
        {
            if(wBlip == null) return;
            
            var position = wBlip.GetPosition();
            position.Z += 2.5f;
            
            const float radius = 0.5f;
            const float height = 0.5f;
            World.DrawMarker(
                MarkerType.UpsideDownCone,
                position,
                Vector3.Zero,
                Vector3.Zero,
                new Vector3(radius, radius, height),
                ColorHelper.GetSystemColor(wBlip.BColor),
                true);
        }
        
    }
}