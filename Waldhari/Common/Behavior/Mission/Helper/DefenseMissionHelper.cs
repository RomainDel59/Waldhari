using System.Collections.Generic;
using GTA;
using Waldhari.Common.Entities;
using Waldhari.Common.Entities.Helpers;
using Waldhari.Common.Files;
using Waldhari.Common.Misc;

namespace Waldhari.Common.Behavior.Mission.Helper
{
    public class DefenseMissionHelper
    {
        private static readonly Dictionary<string,int> NextAttackTry = new Dictionary<string, int>();

        private static string TypeOf<T>()
        {
            return typeof(T).ToString();
        }

        public static void AddCooldown<T>()
        {
            var type = TypeOf<T>();
            
            // DefenseCooldown => in game minutes
            NextAttackTry[type] = Game.GameTime + GlobalOptions.Instance.DefenseCooldown * 60 * 1000;
        }
        
        public static void TryToStart<T>(WPosition property, int product) where T : GenericDefenseMissionScript
        {
            var type = TypeOf<T>();
            
            // First try : add cooldown
            if (!NextAttackTry.TryGetValue(type, out var nextTry))
            {
                AddCooldown<T>();
                return;
            }
            
            // Has not to try
            if (nextTry > Game.GameTime) return;

            // If a mission is active (including defense)
            if (AbstractMissionScript.IsAnyMissionActive() || Game.IsMissionActive || Game.IsRandomEventActive)
            {
                AddCooldown<T>();
                return;
            }
            
            // Is too far : 100 meters
            if (!WPositionHelper.IsNear(Game.Player.Character.Position, property.Position, 100))
            {
                AddCooldown<T>();
                return;
            }
            
            // If no product : no attack
            if (product == 0) return;
            
            // Try to attack
            Logger.Info($"Trying {type}");
            if (RandomHelper.Try(GlobalOptions.Instance.RivalChance))
            {
                var script = Script.InstantiateScript<T>();
                script.Start();
            }

            AddCooldown<T>();
            
        }
        
        
    }
}