using System.Drawing;
using Common.Files;
using GTA;
using GTA.Math;
using GTAVMods.Utils;

namespace Common.Entities
{
    public class WVehicle : AbstractEntity
    {

        public readonly Vehicle GtaVehicle;

        public WVehicle(VehicleHash model, Vector3 position, Vector3 rotation = default)
        {
            GtaVehicle = CreateVehicle(model, position);
            if (GtaVehicle == null)
            {
                Logger.Error($"Could not create vehicle {model}!");
                return;
            }

            GtaVehicle.PositionNoOffset = position;
            GtaVehicle.Rotation = rotation;
            Entity = GtaVehicle;
        }

        private Vehicle CreateVehicle(Model model, Vector3 position)
        {
            model.Request();
            model.Request(10000);

            var vehicle = World.CreateVehicle(model, position);

            model.MarkAsNoLongerNeeded();

            return vehicle;
        }

        public void AttachMissionMarker()
        {
            var position = GtaVehicle.Position;
            position.Z += 2.5f;

            World.DrawMarker(
                MarkerType.UpsideDownCone,
                position,
                Vector3.Zero,
                Vector3.Zero,
                new Vector3(0.5f, 0.5f, 0.5f),
                Color.Yellow);
        }

        public bool IsDestroyed()
        {
            return GtaVehicle.BodyHealth <= 0 || GtaVehicle.EngineHealth <= 0;
        }
    }
}