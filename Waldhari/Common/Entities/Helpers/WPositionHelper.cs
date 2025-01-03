using System;
using GTA;
using GTA.Math;
using Waldhari.Common.Entities;

namespace Waldhari.Common.Misc
{
    public static class WPositionHelper
    {
        // /// <summary>
        // /// Returns a random position on the street behind the player, at a fixed distance.
        // /// </summary>
        // /// <param name="distance">Distance of the random position.</param>
        // /// <returns>Random position behind the player</returns>
        // public static Vector3 GetBehindStreetPosition(int distance)
        // {
        //     var playerPosition = Game.Player.Character.Position;
        //     var playerForwardVector = Game.Player.Character.ForwardVector;
        //     var spawnDistance = distance + RandomHelper.Next(1, 10)*10;
        //     var spawnPosition = playerPosition - (playerForwardVector * spawnDistance);
        //     
        //     return World.GetNextPositionOnStreet(spawnPosition, true);
        // }
        
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
                Rotation = new Vector3(0f, 0f, heading), // rotation sur l'axe Z
                Heading = heading
            };

            return wPosition;
        }


    }
}