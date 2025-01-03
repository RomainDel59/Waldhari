using GTA;
using Waldhari.Common.Files;

namespace Waldhari.Common.Entities.Helpers
{
    public static class WGroupHelper
    {
        
        public static WGroup CreateRivalMembers(int number, bool withVehicles = true)
        {
            var group = new WGroup();
            group.Name = "RivalMembers";
            group.Create();

            for (var i = 0; i < number; i++)
            {
                Logger.Info("Create rival member "+i);
                
                //todo: add a parameter for PedHash
                var wPed = new WPed
                {
                    PedHash = PedHash.Lost01GMY,
                    WBlip = WBlipHelper.GetEnemy("rival_enemy")
                };
                group.AddWPed(wPed, i == 0);
                
                Logger.Info("Rival member created "+i);

                if (!withVehicles) continue;
                
                //todo: add a parameter for VehicleHash
                var wVehicle = new WVehicle
                {
                    VehicleHash = VehicleHash.ZombieA
                };
                group.AddWVehicle(wVehicle);
                
                Logger.Info("Vehicle created "+i);
            }
            
            return group;
        }
        
        
        
    }
}