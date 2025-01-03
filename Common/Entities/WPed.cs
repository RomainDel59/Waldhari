using System.Drawing;
using System.Linq;
using Common.Files;
using GTA;
using GTA.Math;
using GTAVMods.Utils;

namespace Common.Entities
{
    public class WPed : AbstractEntity
    {
        public Ped GtaPed;
        public Vector3 InitialPosition;
        public Vector3 InitialRotation;
        public string InitialScenario;

        public bool hasMoved;
        public bool isGoingBack;
        
        public Vehicle Vehicle;

        public WPed(Ped ped)
        {
            GtaPed = ped;
            Entity = GtaPed;
        }

        public WPed(PedHash model, Vector3 position, Vector3 rotation = default)
        {
            GtaPed = CreatePed(model, position);
            if (GtaPed == null)
            {
                Logger.Error($"Could not create ped {model}!");
                return;
            }

            GtaPed.PositionNoOffset = position;
            if (rotation != default) GtaPed.Rotation = rotation;
            Entity = GtaPed;
        }

        private Ped CreatePed(Model model, Vector3 position)
        {
            model.Request();
            model.Request(10000);

            var ped = World.CreatePed(model, position);

            model.MarkAsNoLongerNeeded();

            return ped;
        }

        public override void Remove()
        {
            if (Vehicle != null)
            {
                Vehicle.Delete();
                Vehicle = null;
            }
            
            base.Remove();
        }
        
        public void GiveWeapons()
        {
            GtaPed.Weapons.Give(WeaponHash.AssaultRifle, 9999, true, true);
            GtaPed.Weapons.Give(WeaponHash.Pistol, 9999, true, true);
            GtaPed.Weapons.Give(WeaponHash.Bat, 1, true, true);
            GtaPed.Weapons.Select(GtaPed.Weapons.BestWeapon);
        }

        public bool GetOnNewVehicle(string modelName, Vector3 position)
        {
            var vehicleModel = new Model(modelName);
            if (!vehicleModel.IsInCdImage || !vehicleModel.IsValid) Logger.Warning($"vehicle {vehicleModel} invalid!");
            
            vehicleModel.Request(500);
            while (!vehicleModel.IsLoaded)
            {
                Script.Wait(1);
            }

            Vehicle = World.CreateVehicle(vehicleModel, position);
            if (Vehicle == null)
            {
                Logger.Warning($"Could not create vehicle {vehicleModel}.");
                return false;
            }
            
            GtaPed.Task.EnterVehicle(Vehicle, VehicleSeat.Driver, -1, 30.0f, EnterVehicleFlags.WarpIn);
            
            var startTime = Game.GameTime;
            while (!GtaPed.IsInVehicle(Vehicle) && Game.GameTime - startTime < 5000)
            {
                Script.Wait(1);
            }
            
            if (GtaPed.IsInVehicle(Vehicle))
            {
                Logger.Debug($"Ped is now in the vehicle {modelName}!");
                return true;
            }
            else
            {
                Logger.Warning($"Ped failed to enter the vehicle {modelName} in time.");
                return false;
            }
            
        }
        
        public void PlayScenario(string scenario)
        {
            GtaPed.Task.StartScenario(scenario, GtaPed.Position.Z);
        }

        public void AttachMissionMarker()
        {
            var position = GtaPed.Position;
            position.Z += 1.5f;

            World.DrawMarker(
                MarkerType.UpsideDownCone,
                position,
                Vector3.Zero,
                Vector3.Zero,
                new Vector3(0.5f, 0.5f, 0.5f),
                Color.Yellow);
        }

        public static WPed GetPed(PedHash pedHash, Vector3 position, bool getNearBy = true)
        {
            WPed wPed;

            // Search the ped nearby
            Ped found = null;
            if (getNearBy)
            {
                var peds = World.GetNearbyPeds(position, 50);
                found = peds.FirstOrDefault(first => first.Model == pedHash);
            }

            // If there is this ped nearby : return it
            if (found != null)
            {
                wPed = new WPed(found);
            }
            // Else : create it
            else
            {
                wPed = new WPed(pedHash, position, Vector3.Zero);
            }

            return wPed;
        }

        public void PlayAnimation(string dictionary, string animation)
        {
            GtaPed.Task.PlayAnimation(
                dictionary,
                animation,
                8f,
                -8f,
                -1,
                AnimationFlags.Loop,
                0f);
        }

        public void Says(string text)
        {
            SoundHelper.PedSays(GtaPed, text);
        }

        public void StayInInitialPosition()
        {
            if (GtaPed == null) return;

            if (GtaPed.IsInCombat) return;

            if (GtaPed.Position.DistanceTo(InitialPosition) > 0.5f)
            {
                if (isGoingBack) return;

                hasMoved = true;
                GtaPed.Task.GoTo(InitialPosition);
                isGoingBack = true;
            }
            else
            {
                isGoingBack = false;

                if (hasMoved)
                {
                    GtaPed.Rotation = InitialRotation;
                    GtaPed.Heading = InitialRotation.Z;
                    if (InitialScenario != null) PlayScenario(InitialScenario);
                    hasMoved = false;
                }
            }
        }

        public void AttachEnemyBlip(string nameKey)
        {
            if (_wBlip != null) return;

            _wBlip = new WBlip(nameKey, false, BlipColor.Red);
            _wBlip.AttachedEntity = Entity;
            _wBlip.Sprite = BlipSprite.Enemy;
            _wBlip.Create();
        }
        
        public void AttachEnemyMarker()
        {
            if (GtaPed == null || GtaPed.IsDead) return;
            
            var position = GtaPed.Position;
            position.Z += 1.5f;

            World.DrawMarker(
                MarkerType.UpsideDownCone,
                position,
                Vector3.Zero,
                Vector3.Zero,
                new Vector3(0.5f, 0.5f, 0.5f),
                Color.Red);
        }

        public void MarkAsNoLongerNeeded()
        {
            if(GtaPed != null) GtaPed.MarkAsNoLongerNeeded();
            if(Vehicle != null) Vehicle.MarkAsNoLongerNeeded();
        }
    }
}