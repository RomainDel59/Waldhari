using System;
using System.Collections.Generic;
using GTA;
using GTA.Math;
using Waldhari.Common.Misc;

namespace Waldhari.Common.Entities.Helpers
{
    public static class WPositionHelper
    {
        
        /// <summary>
        /// Returns a position behind the player, with the rotation facing the player, and the heading.
        /// </summary>
        /// <param name="distance">Distance of the position behind the player.</param>
        /// <param name="onStreet">If true : on the street, otherwise on the sidewalk.</param>
        /// <returns>WPosition with the position, rotation, and heading</returns>
        public static WPosition GetBehindPosition(int distance, bool onStreet)
        {
            var playerPosition = Game.Player.Character.Position;
            var playerForwardVector = Game.Player.Character.ForwardVector;
            var spawnDistance = distance + RandomHelper.Next(1, 10) * 10;
            var spawnPosition = playerPosition - (playerForwardVector * spawnDistance);

            var finalPosition = onStreet
                ? World.GetNextPositionOnStreet(spawnPosition, true)
                : World.GetNextPositionOnSidewalk(spawnPosition);

            // Direction oriented to the player
            var directionToPlayer = playerPosition - finalPosition;
            var heading = (float)Math.Atan2(directionToPlayer.Y, directionToPlayer.X);
            
            var wPosition = new WPosition
            {
                Position = finalPosition,
                Rotation = new Vector3(0f, 0f, heading),
                Heading = heading
            };

            return wPosition;
        }
        
        public static bool IsNear(Vector3 position1, Vector3 position2, float distance)
        {
            return position1.DistanceTo(position2) <= distance;
        }

        private static List<WPosition> MissionWithVehiclePossiblePositions = new List<WPosition>
        {
            // Near Procopio Beach
            new WPosition
            {
                Position = new Vector3(1448.546f, 6548.113f, 15.21889f),
                Rotation = new Vector3(0, 0, 142.5344f)
            }
        };
    }
}