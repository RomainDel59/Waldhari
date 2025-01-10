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
                // East of Procopio Beach at Mount Chiliad
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

                // Behind You Tool Market at San Chianski Mountain
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
                },

                // Sawmill in Paleto Forest
                new MissionWithVehiclePosition
                {
                    VehiclePosition = new WPosition
                    {
                        Position = new Vector3(-582.0705f, 5252.632f, 70.46659f),
                        Rotation = new Vector3(-9.859331E-11f, -1.870987E-09f, -27.69256f),
                        Heading = 332.3074f
                    },
                    PedPositions = new List<WPosition>
                    {
                        new WPosition
                        {
                            Position = new Vector3(-567.702f, 5253.197f, 70.4875f),
                            Rotation = new Vector3(-1.439492E-10f, -1.84389E-09f, 69.16079f),
                            Heading = 69.16079f
                        },
                        new WPosition
                        {
                            Position = new Vector3(-567.0833f, 5269.567f, 70.29375f),
                            Rotation = new Vector3(-2.10556E-10f, -1.673166E-09f, 153.1062f),
                            Heading = 153.1062f
                        },
                        new WPosition
                        {
                            Position = new Vector3(-569.7237f, 5245.887f, 70.46827f),
                            Rotation = new Vector3(-2.274333E-10f, -1.650104E-09f, 20.90506f),
                            Heading = 20.90506f
                        }
                    }
                },

                // Beach of North Chumash
                new MissionWithVehiclePosition
                {
                    VehiclePosition = new WPosition
                    {
                        Position = new Vector3(-2417.603f, 4221.702f, 9.715014f),
                        Rotation = new Vector3(0f, 0f, -48.43934f),
                        Heading = 311.5607f
                    },
                    PedPositions = new List<WPosition>
                    {
                        new WPosition
                        {
                            Position = new Vector3(-2404.96f, 4244.402f, 9.947919f),
                            Rotation = new Vector3(0f, 0f, 141.8307f),
                            Heading = 141.8307f
                        },
                        new WPosition
                        {
                            Position = new Vector3(-2393.339f, 4248.593f, 11.12685f),
                            Rotation = new Vector3(0f, 0f, 102.1449f),
                            Heading = 102.1449f
                        },
                        new WPosition
                        {
                            Position = new Vector3(-2400.642f, 4255.182f, 9.926501f),
                            Rotation = new Vector3(0f, 0f, 151.3061f),
                            Heading = 151.3061f
                        }
                    }
                },

                // Between sheds in the North of Grapeseed
                new MissionWithVehiclePosition
                {
                    VehiclePosition = new WPosition
                    {
                        Position = new Vector3(1970.395f, 5178.13f, 47.83459f),
                        Rotation = new Vector3(-0.4519052f, 0.5442668f, 136.1888f),
                        Heading = 136.1869f
                    },
                    PedPositions = new List<WPosition>
                    {
                        new WPosition
                        {
                            Position = new Vector3(1968.858f, 5168.964f, 47.63907f),
                            Rotation = new Vector3(-0.4519716f, 0.5442787f, 109.6198f),
                            Heading = 109.6196f
                        },
                        new WPosition
                        {
                            Position = new Vector3(1959.101f, 5173.365f, 47.97238f),
                            Rotation = new Vector3(-0.4566186f, 0.5463271f, -179.6217f),
                            Heading = 180.3739f
                        },
                        new WPosition
                        {
                            Position = new Vector3(1962.188f, 5177.959f, 47.93384f),
                            Rotation = new Vector3(-0.4566464f, 0.5463446f, -78.0014f),
                            Heading = 281.9986f
                        }
                    }
                },

                // Marlowe Vineyards in Tongva Hills
                new MissionWithVehiclePosition
                {
                    VehiclePosition = new WPosition
                    {
                        Position = new Vector3(-1920.249f, 2048.372f, 140.7355f),
                        Rotation = new Vector3(0f, 0f, -116.9237f),
                        Heading = 243.0763f
                    },
                    PedPositions = new List<WPosition>
                    {
                        new WPosition
                        {
                            Position = new Vector3(-1899.844f, 2055.413f, 140.8463f),
                            Rotation = new Vector3(0f, 0f, 163.4475f),
                            Heading = 163.4475f
                        },
                        new WPosition
                        {
                            Position = new Vector3(-1898.34f, 2045.104f, 140.911f),
                            Rotation = new Vector3(0f, 0f, 83.23779f),
                            Heading = 83.23779f
                        },
                        new WPosition
                        {
                            Position = new Vector3(-1922.514f, 2060.218f, 140.8334f),
                            Rotation = new Vector3(0f, 0f, -90.92151f),
                            Heading = 269.0785f
                        }
                    }
                },

                // Palmer-Taylor Power Station parking at San Chianski Mountain
                new MissionWithVehiclePosition
                {
                    VehiclePosition = new WPosition
                    {
                        Position = new Vector3(2661.113f, 1667.477f, 24.48861f),
                        Rotation = new Vector3(-0.000118997f, 0.0001710464f, -92.47883f),
                        Heading = 267.5212f
                    },
                    PedPositions = new List<WPosition>
                    {
                        new WPosition
                        {
                            Position = new Vector3(2662.809f, 1656.169f, 24.50488f),
                            Rotation = new Vector3(-0.0001189836f, 0.0001710686f, -53.85175f),
                            Heading = 306.1483f
                        },
                        new WPosition
                        {
                            Position = new Vector3(2679.935f, 1662.141f, 24.56851f),
                            Rotation = new Vector3(-0.0001189185f, 0.0001710819f, 61.19814f),
                            Heading = 61.19814f
                        },
                        new WPosition
                        {
                            Position = new Vector3(2682.33f, 1679.312f, 24.54166f),
                            Rotation = new Vector3(-0.0001189309f, 0.0001710892f, 157.1019f),
                            Heading = 157.1019f
                        }
                    }
                },

                // Murrieta Oil Field in El Burro Height
                new MissionWithVehiclePosition
                {
                    VehiclePosition = new WPosition
                    {
                        Position = new Vector3(1678.122f, -1845.062f, 109.3372f),
                        Rotation = new Vector3(0f, 0f, -97.68655f),
                        Heading = 262.3134f
                    },
                    PedPositions = new List<WPosition>
                    {
                        new WPosition
                        {
                            Position = new Vector3(1675.642f, -1837.28f, 109.6372f),
                            Rotation = new Vector3(0f, 0f, -116.048f),
                            Heading = 243.952f
                        },
                        new WPosition
                        {
                            Position = new Vector3(1695.931f, -1831.175f, 109.8533f),
                            Rotation = new Vector3(0f, 0f, 116.2625f),
                            Heading = 116.2625f
                        },
                        new WPosition
                        {
                            Position = new Vector3(1701.745f, -1839.845f, 109.3042f),
                            Rotation = new Vector3(0f, 0f, 85.80251f),
                            Heading = 85.80251f
                        }
                    }
                },

                // Shed in Rogers Salvage & Scrap at La Puerta
                new MissionWithVehiclePosition
                {
                    VehiclePosition = new WPosition
                    {
                        Position = new Vector3(-426.7242f, -1692.686f, 19.02909f),
                        Rotation = new Vector3(0f, 0f, 160.8085f),
                        Heading = 160.8085f
                    },
                    PedPositions = new List<WPosition>
                    {
                        new WPosition
                        {
                            Position = new Vector3(-435.2992f, -1697.45f, 18.96523f),
                            Rotation = new Vector3(0f, 0f, 172.6006f),
                            Heading = 172.6006f
                        },
                        new WPosition
                        {
                            Position = new Vector3(-422.285f, -1698.454f, 19.07702f),
                            Rotation = new Vector3(0f, 0f, 73.47246f),
                            Heading = 73.47246f
                        },
                        new WPosition
                        {
                            Position = new Vector3(-428.8332f, -1682.184f, 19.02909f),
                            Rotation = new Vector3(0f, 0f, -175.7283f),
                            Heading = 184.2717f
                        }
                    }
                }
            };

        #endregion
    }
}