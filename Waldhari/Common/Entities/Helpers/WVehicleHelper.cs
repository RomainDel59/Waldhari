using GTA;
using Waldhari.Common.Files;

namespace Waldhari.Common.Entities.Helpers
{
    public static class WVehicleHelper
    {
        
        /// <summary>
        /// Makes a ped wrap into a vehicle and wait for it.
        /// </summary>
        /// <param name="wPed">Ped that has to enter</param>
        /// <param name="wVehicle">Vehicle that the ped has to enter</param>
        /// <returns>Ped wrapped into the vehicle</returns>
        public static bool MakePedWarpInVehicle(WPed wPed, WVehicle wVehicle)
        {
            wPed.Ped.Task.EnterVehicle(wVehicle.Vehicle, VehicleSeat.Any, -1, 30.0f, EnterVehicleFlags.WarpIn);
            
            // Script wait ped to enter vehicle
            var startTime = Game.GameTime;
            while (!wPed.Ped.IsInVehicle(wVehicle.Vehicle) && Game.GameTime - startTime < 2000)
            {
                Script.Wait(1);
            }

            if (wPed.Ped.IsInVehicle(wVehicle.Vehicle)) return true;
            
            Logger.Warning($"Ped {wPed.Ped.Handle} failed to wrap into the vehicle {wVehicle.Vehicle.Handle} in 2 seconds.");
            
            return false;
        }
        
        
    }
}