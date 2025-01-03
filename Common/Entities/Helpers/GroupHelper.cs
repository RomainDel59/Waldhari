using Common.Files;
using Common.Misc;
using GTA;
using GTAVMods.Utils;

namespace Common.Entities.Helpers
{
    public static class GroupHelper
    {
        //todo: move this param to WGroup, and move the member whole creation on WGroup.Add
        public static int AppearanceDistance = -1;
        
        public static WGroup CreateRivalMembers(int number, bool withVehicles = true)
        {
            if(AppearanceDistance == -1) throw new TechnicalException("Appearance distance is not defined");
            
            var group = new WGroup();
            group.Create("RivalMembers");

            for (var i = 0; i < number; i++)
            {
                Logger.Debug("Create rival member "+i);
                
                var position = PositionHelper.GetBehindStreetPosition(AppearanceDistance);
                
                //todo: add a parameter for PedHash
                var member = new WPed(PedHash.Lost01GMY, World.GetNextPositionOnSidewalk(position));
                if(member.GtaPed == null)
                {
                    Logger.Warning("Could not create rival member.");
                    continue;
                }
                
                group.Add(member, i == 0);
                member.AttachEnemyBlip("rival_enemy");
                member.GiveWeapons();
                member.GtaPed.IsEnemy = true;
                member.GtaPed.CanSwitchWeapons = true;
                member.GtaPed.NeverLeavesGroup = true;

                //todo: add a parameter for vehicle model
                if (withVehicles && member.GetOnNewVehicle("zombiea", position))
                {
                    member.GtaPed.Task.VehicleChase(Game.Player.Character);                    
                }
                else
                {
                    member.GtaPed.Task.FightAgainst(Game.Player.Character);
                }
                
                Logger.Debug("Rival member created "+i);
            }
            
            return group;
        }
        
        
        
    }
}