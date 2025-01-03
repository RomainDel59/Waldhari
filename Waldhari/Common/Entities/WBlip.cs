using System.Drawing;
using GTA;
using GTA.Math;
using Waldhari.Common.Files;

namespace Waldhari.Common.Entities
{
    public class WBlip
    {
        public Blip GtaBlip;

        private readonly string _nameKey;
        private readonly bool _showRoute;
        private readonly BlipColor _blipColor;

        public Vector3 Position;
        public BlipSprite Sprite;
        public bool IsShortRange;
        public Entity AttachedEntity;

        public WBlip(
            string nameKey,
            bool showRoute = true,
            BlipColor blipColor = BlipColor.Yellow
        )
        {
            _nameKey = nameKey;
            _blipColor = blipColor;
            _showRoute = showRoute;
        }

        public WBlip(
            BlipSprite sprite,
            Vector3 position)
        {
            Position = position;
            Sprite = sprite;
        }

        public void CreateOnEntity(Entity entity)
        {
            AttachedEntity = entity;
            GtaBlip = AttachedEntity.AddBlip();
            if (GtaBlip == null)
            {
                Logger.Warning("Blip could not be created on entity");
                return;
            }

            Complete();
        }

        public void CreateAtPosition(Vector3 position)
        {
            Position = position;
            GtaBlip = World.CreateBlip(Position);
            if (GtaBlip == null)
            {
                Logger.Warning("Blip could not be created at position");
                return;
            }

            Complete();
        }

        public void Create()
        {
            if (GtaBlip != null) return;

            if (AttachedEntity != null)
            {
                CreateOnEntity(AttachedEntity);
            }
            else if (Position != Vector3.Zero)
            {
                CreateAtPosition(Position);
            }
            else
            {
                Logger.Warning("Attempted to recreate a Blip without definition.");
            }
        }

        public void AttachMissionMarkerToPositionBlip()
        {
            if (GtaBlip == null || Position == default) return;

            var position = Position;
            position.Z = World.GetGroundHeight(position);

            World.DrawMarker(
                MarkerType.VerticalCylinder,
                position,
                Vector3.Zero,
                Vector3.Zero,
                new Vector3(5, 5, 0.5f),
                Color.Yellow);
        }

        private void Complete()
        {
            if (Sprite != default) GtaBlip.Sprite = Sprite;
            if (_blipColor != default) GtaBlip.Color = _blipColor;
            if (_nameKey != null) GtaBlip.Name = Localization.GetTextByKey(_nameKey);
            GtaBlip.ShowRoute = _showRoute;
            GtaBlip.IsShortRange = IsShortRange;
        }

        public void Remove()
        {
            if (GtaBlip == null) return;

            GtaBlip.Delete();
            GtaBlip = null;
        }
    }
}