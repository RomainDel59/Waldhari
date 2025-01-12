using System.Collections.Generic;
using GTA;
using Waldhari.Common.Behavior.Ped;

namespace Waldhari.Common.Entities.Helpers
{
    public static class WSceneHelper
    {
        public static List<PedActingScript> CreateTransactionScene(WPositionHelper.MissionWithVehiclePosition position, WGroupHelper.Gang gang)
        {
            var result = new List<PedActingScript>();

            var chief = new WPed
            {
                PedHash = gang.GetRandomPedHash(),
                InitialPosition = position.PedPositions[0],
                Scenario = WPed.PedScenario.Smoking
            };
            chief.Create();
            chief.AddWeapon(WeaponsHelper.GetRandomGangWeapon());

            result.Add(Script.InstantiateScript<PedActingScript>());
            result[0].WPed = chief;

            for(var i = 1; i <= 2; i++)
            {
                var guard = new WPed
                {
                    PedHash = gang.GetRandomPedHash(),
                    InitialPosition = position.PedPositions[i],
                    Scenario = WPed.PedScenario.Guard
                };
                guard.Create();
                guard.AddWeapon(WeaponsHelper.GetRandomGangWeapon());
                guard.AddWeapon(WeaponsHelper.GetRandomGangWeapon());
                result.Add(Script.InstantiateScript<PedActingScript>());
                result[i].WPed = guard;
            }
            
            return result;
        }
    }
}