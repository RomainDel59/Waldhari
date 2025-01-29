using System.Collections.Generic;
using GTA;
using Waldhari.Common.Files;
using Waldhari.Common.Misc;

namespace Waldhari.Common.Entities.Helpers
{
    public static class WVehicleHelper
    {
        private static List<VehicleHash> _vanHashes = new List<VehicleHash>
        {
            VehicleHash.Burrito,
            VehicleHash.Burrito2,
            VehicleHash.Burrito3,
            VehicleHash.Burrito4,
            VehicleHash.Burrito5,
            VehicleHash.Pony,
            VehicleHash.Pony2,
            VehicleHash.Speedo,
            VehicleHash.Speedo2,
            VehicleHash.Speedo4,
            VehicleHash.Youga2,
            VehicleHash.Youga3,
            VehicleHash.Youga4
        };

        public static VehicleHash GetRandomVan()
        {
            var index = RandomHelper.Next(0, _vanHashes.Count);
            return _vanHashes[index];
        }
        
        /// <summary>
        /// Makes a ped wrap into a vehicle and wait for it.
        /// </summary>
        /// <param name="wPed">Ped that has to enter</param>
        /// <param name="wVehicle">Vehicle that the ped has to enter</param>
        /// <returns>Ped wrapped into the vehicle</returns>
        public static bool MakePedWarpInVehicle(WPed wPed, WVehicle wVehicle)
        {
            var seat = wVehicle.Vehicle.IsSeatFree(VehicleSeat.Driver) ? VehicleSeat.Driver : VehicleSeat.Any;

            //speed : 1.0 = walk, 2.0 = run see https://docs.fivem.net/natives/?_0xC20E50AA46D09CA8
            wPed.Ped.Task.EnterVehicle(wVehicle.Vehicle, seat, -1, 2, EnterVehicleFlags.WarpIn);
            
            // Script waits 2 seconds that ped enters vehicle
            var timeOut = Game.GameTime + 2000;
            while (!wPed.Ped.IsInVehicle(wVehicle.Vehicle) && timeOut > Game.GameTime)
            {
                Script.Wait(1);
                wPed.Ped.Task.EnterVehicle(wVehicle.Vehicle, seat, -1, 2, EnterVehicleFlags.WarpIn);
            }

            if (wPed.Ped.IsInVehicle(wVehicle.Vehicle)) return true;
            
            Logger.Warning($"Ped {wPed.Ped.Handle} failed to wrap into the vehicle {wVehicle.Vehicle.Handle} in 2 seconds.");
            
            return false;
        }
        
        
    }
}