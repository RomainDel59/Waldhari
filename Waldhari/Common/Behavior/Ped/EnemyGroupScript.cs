using System;
using GTA;
using Waldhari.Common.Entities;
using Waldhari.Common.Entities.Helpers;
using Waldhari.Common.Exceptions;
using Waldhari.Common.Files;
using Waldhari.Common.UI;

namespace Waldhari.Common.Behavior.Ped
{
    [ScriptAttributes(NoDefaultInstance = true)]
    public class EnemyGroupScript : Script
    {
        private int _nextExecution = Game.GameTime;
        
        // Parameters should be defined when game is launched
        public static int AppearanceDistance = -1;
        public static int DisappearanceDistance = -1;
        
        /// <summary>
        /// Group has to be defined on DefineGroup method.
        /// </summary>
        public WGroup WGroup;

        /// <summary>
        /// Create a script that will be executed until disinstantiation.
        /// Group has to be defined and created.
        /// Peds, vehicles and blips have to be defined, but NOT created.
        /// Peds and vehicles position will be calculated when created in this script.
        /// Peds weapons will be given when created in this script.
        /// At the end, before disinstantiation of the script,
        /// developer has to call methods MarkAsNoLongerNeeded or Remove to clean up.
        /// </summary>
        /// <exception cref="TechnicalException">If distance parameters are empty</exception>
        public EnemyGroupScript()
        {
            if(AppearanceDistance == -1) throw new TechnicalException("AppearanceDistance cannot be empty.");
            if(DisappearanceDistance == -1) throw new TechnicalException("DisappearanceDistance cannot be empty.");

            Tick += OnTick;
        }

        public void DefineGroup(WGroup wGroup)
        {
            CreateVehicles(wGroup);
            CreatePeds(wGroup);
            WarpInVehicle(wGroup);
            FightAgainstPlayer(wGroup);
            WGroup = wGroup;
        }

        /// <summary>
        /// Marks peds and vehicles as no longer needed,
        /// so game can remove its freely when it wants.
        /// </summary>
        public void MarkAsNoLongerNeeded()
        {
            WGroup?.MarkAsNoLongerNeeded();
            WGroup?.ClearTask();
        }

        /// <summary>
        /// Remove all peds and vehicles (delete everything).
        /// </summary>
        public void Remove()
        {
            foreach (var wPed in WGroup.WPeds)
            {
                if (wPed == null) continue;
                
                wPed.WBlip?.Remove();
                wPed.Remove();
            }
            
            if (!WGroup.HasVehicles()) return;
            
            foreach (var wVehicle in WGroup.WVehicles)
            {
                if (wVehicle == null) continue;
                
                wVehicle.Remove();
            }
        }
        
        /// <summary>
        /// Creates vehicles according group definition.
        /// If there is no vehicle, does nothing.
        /// </summary>
        private void CreateVehicles(WGroup wGroup)
        {
            if (!wGroup.HasVehicles()) return;
            
            foreach (var wVehicle in wGroup.WVehicles)
            {
                wVehicle.InitialPosition = WPositionHelper.GetBehindPosition(AppearanceDistance, true);
                wVehicle.Create();
            }
        }

        /// <summary>
        /// Creates peds according group definition.
        /// </summary>
        private void CreatePeds(WGroup wGroup)
        {
            if (wGroup.WPeds == null || wGroup.WPeds.Count == 0) return;

            var hasVehicles = wGroup.HasVehicles();

            var pedIndex = 0;
            foreach (var wPed in wGroup.WPeds)
            {
                if (wPed == null) continue;
                
                wPed.InitialPosition = WPositionHelper.GetBehindPosition(AppearanceDistance, false);
                wPed.Create();
                wPed.WBlip.Ped = wPed.Ped;
                wPed.WBlip.Create();

                // give 2 weapons
                for(var i = 0; i < 2; i++)
                {
                    var weapon = hasVehicles
                        ? WeaponsHelper.GetRandomGangVehicleWeapon()
                        : WeaponsHelper.GetRandomGangWeapon();
                    wPed.AddWeapon(weapon);
                }
                
                wGroup.AddInGroup(wPed, pedIndex == 0);

                pedIndex++;
            }
            
        }

        private void WarpInVehicle(WGroup wGroup)
        {
            if (!wGroup.HasVehicles()) return;

            var vehicleNumber = wGroup.WVehicles.Count;
            var vehicleIndex = 0;
            foreach (var wPed in wGroup.WPeds)
            {
                if(vehicleIndex >= vehicleNumber) vehicleIndex = 0;
                
                var vehicle = wGroup.WVehicles[vehicleIndex];
                GetOnVehicle(wPed, vehicle);
                
                vehicleIndex++;
            }

        }

        private void FightAgainstPlayer(WGroup wGroup)
        {
            var hasVehicles = wGroup.HasVehicles();

            foreach (var wPed in wGroup.WPeds)
            {
                wPed.Ped.Task.FightAgainst(Game.Player.Character);
                if(hasVehicles) wPed.Ped.Task.VehicleChase(Game.Player.Character);
            }
        }

        /// <summary>
        /// Executes peds and vehicles behavior every 1/2 second.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnTick(object sender, EventArgs e)
        {
            // Wait for parameter
            if (WGroup == null) return;
            
            ManagePeds();
            ManageVehicles();
            
        }

        /// <summary>
        /// Manages vehicles behavior.
        /// If there is no vehicle, does nothing.
        /// </summary>
        private void ManageVehicles()
        {
            if(!WGroup.HasVehicles()) return;
            
            foreach (var wVehicle in WGroup.WVehicles)
            {
                if (wVehicle == null) continue;
                if (wVehicle.Vehicle == null) continue;
                
                // If vehicle is too far
                if (wVehicle.Vehicle.Position.DistanceTo(Game.Player.Character.Position) > DisappearanceDistance)
                {
                    wVehicle.Remove();
                    continue;
                }
                
                // If vehicle is not destroyed yet
                if (!wVehicle.Vehicle.IsConsideredDestroyed) continue;
                
                // Vehicle destroyed : no longer needed
                wVehicle.Vehicle.MarkAsNoLongerNeeded();
            }
        }

        /// <summary>
        /// Manages peds behavior.
        /// </summary>
        private void ManagePeds()
        {
            foreach (var wPed in WGroup.WPeds)
            {
                if (wPed == null) continue;
                if (wPed.Ped == null) continue;
                
                // Mark the enemy
                MarkerHelper.DrawEntityMarkerOnBlip(wPed.WBlip);
                
                // If enemy is too far
                if (wPed.Ped.Position.DistanceTo(Game.Player.Character.Position) > DisappearanceDistance)
                {
                    wPed.Remove();
                    continue;
                }
                
                // If enemy is not dead yet
                if (!wPed.Ped.IsDead) continue;
                
                // Enemy dead : no longer needed
                wPed.WBlip?.Remove();
                wPed.WBlip = null;
                wPed.Ped.MarkAsNoLongerNeeded();
                wPed.Ped = null;
            }
        }

        /// <summary>
        /// Make a ped enter a random vehicle within 2 seconds.
        /// If there is no vehicle, does nothing.
        /// </summary>
        /// <param name="wPed">Ped that has to enter vehicle</param>
        /// <returns>True if ped succeed to enter vehicle</returns>
        private bool GetOnVehicle(WPed wPed, WVehicle wVehicle)
        {
            var timeOut = Game.GameTime + 2000;
            
            Logger.Debug($"wPed.Ped = {wPed.Ped}");
            
            // While ped is not in a vehicle and timeOut not reach
            while (!wPed.Ped.IsInVehicle() && timeOut > Game.GameTime)
            {
                Logger.Debug($"wVehicle = {wVehicle}");
                Logger.Debug($"wVehicle.Vehicle = {wVehicle.Vehicle}");
                
                WVehicleHelper.MakePedWarpInVehicle(wPed, wVehicle);
            }
            
            return wPed.Ped.IsInVehicle();
        }
        
    }
}