using Common.Math;
using GTA;
using GTA.Math;

namespace Common.Misc
{
    public static class PositionHelper
    {
        /// <summary>
        /// Returns a random position on the street behind the player, at a fixed distance.
        /// </summary>
        /// <param name="distance">Distance of the random position.</param>
        /// <returns>Random position behind the player</returns>
        public static Vector3 GetBehindStreetPosition(int distance)
        {
            var playerPosition = Game.Player.Character.Position;
            var playerForwardVector = Game.Player.Character.ForwardVector;
            var spawnDistance = distance + RandomHelper.Next(1, 10)*10;
            var spawnPosition = playerPosition - (playerForwardVector * spawnDistance);
            
            return World.GetNextPositionOnStreet(spawnPosition, true);
        }
    }
}