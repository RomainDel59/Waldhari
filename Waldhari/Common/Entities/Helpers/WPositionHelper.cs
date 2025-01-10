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

        #region MissionWithVehiclePossiblePositions
        
        public class MissionWithVehiclePosition
        {
            public WPosition VehiclePosition;
            // First (index=0) is always where mission leads to
            public List<WPosition> PedPositions;
        }

        private static int _lastGivenMissionWithVehiclePosition = -1;
        public static MissionWithVehiclePosition GetRandomMissionWithVehiclePosition()
        {
            var index = RandomHelper.Next(0, MissionWithVehiclePositions.Count);
            while (_lastGivenMissionWithVehiclePosition == index)
            {
                index = RandomHelper.Next(0, MissionWithVehiclePositions.Count);
            }
            _lastGivenMissionWithVehiclePosition = index;
            return MissionWithVehiclePositions[index];
        }

        private static readonly List<MissionWithVehiclePosition> MissionWithVehiclePositions =
            new List<MissionWithVehiclePosition>
            {
                // East of Procopio Beach (Mount Chiliad)
                new MissionWithVehiclePosition
                {
                    VehiclePosition = new WPosition
                    {
                        Position = new Vector3(1445.666f, 6553.742f, 15.03966f),
                        Rotation = new Vector3(-9.646064f, -9.953696f, 135.6729f),
                        Heading = 135.6729f
                    },
                    PedPositions = new List<WPosition>
                    {
                        new WPosition
                        {
                            Position = new Vector3(1452.894f, 6548.653f, 15.00896f),
                            Rotation = new Vector3(-9.673152f, -9.953592f, 115.3211f),
                            Heading = 115.3211f
                        },
                        new WPosition
                        {
                            Position = new Vector3(1463.143f, 6548.165f, 14.30686f),
                            Rotation = new Vector3(-9.666907f, -9.961837f, 75.50282f),
                            Heading = 75.50282f
                        },
                        new WPosition
                        {
                            Position = new Vector3(1461.916f, 6540.115f, 14.68128f),
                            Rotation = new Vector3(-9.681112f, -9.963818f, 64.76723f),
                            Heading = 64.76723f
                        }
                    }
                },

                // Behind You Tool Market (San Chianski Mountain)
                new MissionWithVehiclePosition
                {
                    VehiclePosition = new WPosition
                    {
                        Position = new Vector3(2673.446f, 3521.807f, 52.71204f),
                        Rotation = new Vector3(0, 0, -10.99092f),
                        Heading = 349.0091f
                    },
                    PedPositions = new List<WPosition>
                    {
                        new WPosition
                        {
                            Position = new Vector3(2665.84f, 3512.829f, 53.07535f),
                            Rotation = new Vector3(0, 0, 5.769825f),
                            Heading = 5.769825f
                        },
                        new WPosition
                        {
                            Position = new Vector3(2673.035f, 3508.148f, 52.91177f),
                            Rotation = new Vector3(0, 0, 13.00812f),
                            Heading = 13.00812f
                        },
                        new WPosition
                        {
                            Position = new Vector3(2678.551f, 3517.803f, 52.71204f),
                            Rotation = new Vector3(0, 0, 66.69183f),
                            Heading = 66.69182f
                        }
                    }
                }
                
                
            };
        
        #endregion
    }
}