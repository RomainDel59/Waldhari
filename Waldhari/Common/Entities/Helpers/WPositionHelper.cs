using System;
using System.Collections.Generic;
using System.Linq;
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

        public static bool IsNearPlayer(Vector3 position, float distance)
        {
            return IsNear(Game.Player.Character.Position, position, distance);
        }

        #region MissionWithVehiclePossiblePositions

        public class MissionWithVanPosition
        {
            public WPosition VehiclePosition;

            // First (index=0) is always where mission leads to
            public List<WPosition> PedPositions;
        }

        private static int _lastGivenMissionWithVanPosition = -1;

        public static MissionWithVanPosition GetRandomMissionWithVanPosition()
        {
            var index = RandomHelper.Next(0, MissionWithVanPositions.Count);
            while (_lastGivenMissionWithVanPosition == index)
            {
                index = RandomHelper.Next(0, MissionWithVanPositions.Count);
            }

            _lastGivenMissionWithVanPosition = index;
            return MissionWithVanPositions[index];
        }

        private static readonly List<MissionWithVanPosition> MissionWithVanPositions =
            new List<MissionWithVanPosition>
            {
                // East of Procopio Beach at Mount Chiliad
                new MissionWithVanPosition
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
                new MissionWithVanPosition
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
                new MissionWithVanPosition
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
                new MissionWithVanPosition
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
                new MissionWithVanPosition
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
                new MissionWithVanPosition
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
                new MissionWithVanPosition
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
                new MissionWithVanPosition
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
                new MissionWithVanPosition
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
                },
                
                // Davis Quartz terraced mining site
                new MissionWithVanPosition
                {
                    VehiclePosition = new WPosition
                    {
                        Position = new Vector3(2682.076f, 2805.158f, 40.36162f),
                        Rotation = new Vector3(0f, 0f, 1.757499f),
                        Heading = 1.757499f
                    },
                    PedPositions = new List<WPosition>
                    {
                        new WPosition
                        {
                            Position = new Vector3(2681.902f, 2797.468f, 40.44386f),
                            Rotation = new Vector3(0f, 0f, 10.85631f),
                            Heading = 10.85631f
                        },
                        new WPosition
                        {
                            Position = new Vector3(2687.112f, 2797.593f, 40.36722f),
                            Rotation = new Vector3(0f, 0f, 17.04746f),
                            Heading = 17.04746f
                        },
                        new WPosition
                        {
                            Position = new Vector3(2677.491f, 2797.953f, 40.39575f),
                            Rotation = new Vector3(0f, 0f, -11.99793f),
                            Heading = 348.0021f
                        }
                    }
                },
                
                // A trail in Mount Gordo
                new MissionWithVanPosition
                {
                    VehiclePosition = new WPosition
                    {
                        Position = new Vector3(3334.438f, 5473.732f, 19.88513f),
                        Rotation = new Vector3(0f, 0f, 157.6406f),
                        Heading = 157.6406f
                    },
                    PedPositions = new List<WPosition>
                    {
                        new WPosition
                        {
                            Position = new Vector3(3342.271f, 5461.542f, 20.73017f),
                            Rotation = new Vector3(0f, 0f, 120.9349f),
                            Heading = 120.9349f
                        },
                        new WPosition
                        {
                            Position = new Vector3(3344.042f, 5456.47f, 20.88776f),
                            Rotation = new Vector3(0f, 0f, 128.992f),
                            Heading = 128.992f
                        },
                        new WPosition
                        {
                            Position = new Vector3(3349.472f, 5464.07f, 21.39451f),
                            Rotation = new Vector3(0f, 0f, 98.73433f),
                            Heading = 98.73433f
                        }
                    }
                },
                
                // Palomino Highlands
                new MissionWithVanPosition
                {
                    VehiclePosition = new WPosition
                    {
                        Position = new Vector3(2718.432f, -779.784f, 24.17901f),
                        Rotation = new Vector3(0f, 0f, 169.9973f),
                        Heading = 169.9973f
                    },
                    PedPositions = new List<WPosition>
                    {
                        new WPosition
                        {
                            Position = new Vector3(2711.501f, -771.4313f, 24.48149f),
                            Rotation = new Vector3(0f, 0f, -165.049f),
                            Heading = 194.951f
                        },
                        new WPosition
                        {
                            Position = new Vector3(2731.539f, -784.5287f, 24.41152f),
                            Rotation = new Vector3(0f, 0f, 115.183f),
                            Heading = 115.183f
                        },
                        new WPosition
                        {
                            Position = new Vector3(2733.696f, -776.1342f, 23.79521f),
                            Rotation = new Vector3(0f, 0f, 113.2288f),
                            Heading = 113.2288f
                        }
                    }
                },
                
                // Buccaneer way
                new MissionWithVanPosition
                {
                    VehiclePosition = new WPosition
                    {
                        Position = new Vector3(969.9266f, -3157.754f, 5.900806f),
                        Rotation = new Vector3(0f, 0f, 178.676f),
                        Heading = 178.676f
                    },
                    PedPositions = new List<WPosition>
                    {
                        new WPosition
                        {
                            Position = new Vector3(963.4526f, -3152.228f, 5.900805f),
                            Rotation = new Vector3(0f, 0f, 170.5244f),
                            Heading = 170.5244f
                        },
                        new WPosition
                        {
                            Position = new Vector3(961.2641f, -3149.925f, 5.900805f),
                            Rotation = new Vector3(0f, 0f, 168.7223f),
                            Heading = 168.7223f
                        },
                        new WPosition
                        {
                            Position = new Vector3(965.5747f, -3150.236f, 5.900805f),
                            Rotation = new Vector3(0f, 0f, 174.9723f),
                            Heading = 174.9723f
                        }
                    }
                },
                
                // Elysian Island
                new MissionWithVanPosition
                {
                    VehiclePosition = new WPosition
                    {
                        Position = new Vector3(528.3453f, -3048.111f, 6.069633f),
                        Rotation = new Vector3(0.0001032013f, -2.623022E-05f, -79.03922f),
                        Heading = 280.9608f
                    },
                    PedPositions = new List<WPosition>
                    {
                        new WPosition
                        {
                            Position = new Vector3(535.9063f, -3052.429f, 6.069633f),
                            Rotation = new Vector3(0.0001033175f, -3.138557E-05f, 7.332377f),
                            Heading = 7.332377f
                        },
                        new WPosition
                        {
                            Position = new Vector3(516.076f, -3051.262f, 6.069631f),
                            Rotation = new Vector3(0.0001033338f, -3.138251E-05f, -84.93154f),
                            Heading = 275.0685f
                        },
                        new WPosition
                        {
                            Position = new Vector3(515.8767f, -3048.829f, 6.069631f),
                            Rotation = new Vector3(0.000103332f, -3.138409E-05f, -73.95679f),
                            Heading = 286.0432f
                        }
                    }
                },
                
                // Elysian Island 2
                new MissionWithVanPosition
                {
                    VehiclePosition = new WPosition
                    {
                        Position = new Vector3(259.0757f, -2829.338f, 6.020677f),
                        Rotation = new Vector3(0f, 0f, 167.6358f),
                        Heading = 167.6358f
                    },
                    PedPositions = new List<WPosition>
                    {
                        new WPosition
                        {
                            Position = new Vector3(250.2873f, -2830.109f, 6.039652f),
                            Rotation = new Vector3(0f, 0f, -167.0082f),
                            Heading = 192.9918f
                        },
                        new WPosition
                        {
                            Position = new Vector3(251.8873f, -2826.098f, 6.060141f),
                            Rotation = new Vector3(0f, 0f, -22.0651f),
                            Heading = 337.9349f
                        },
                        new WPosition
                        {
                            Position = new Vector3(252.9023f, -2828.257f, 6.037044f),
                            Rotation = new Vector3(0f, 0f, -138.3253f),
                            Heading = 221.6747f
                        }
                    }
                },
                
                // Airport
                new MissionWithVanPosition
                {
                    VehiclePosition = new WPosition
                    {
                        Position = new Vector3(-960.4897f, -2607.395f, 13.83099f),
                        Rotation = new Vector3(0f, 0f, 61.26441f),
                        Heading = 61.26441f
                    },
                    PedPositions = new List<WPosition>
                    {
                        new WPosition
                        {
                            Position = new Vector3(-961.7932f, -2610.471f, 13.98079f),
                            Rotation = new Vector3(0f, 0f, 45.96809f),
                            Heading = 45.96809f
                        },
                        new WPosition
                        {
                            Position = new Vector3(-960.4852f, -2616.772f, 13.9808f),
                            Rotation = new Vector3(0f, 0f, 167.0476f),
                            Heading = 167.0476f
                        },
                        new WPosition
                        {
                            Position = new Vector3(-960.5225f, -2615.056f, 13.9808f),
                            Rotation = new Vector3(0f, 0f, 15.14495f),
                            Heading = 15.14495f
                        }
                    }
                },
                
                // Vespucci
                new MissionWithVanPosition
                {
                    VehiclePosition = new WPosition
                    {
                        Position = new Vector3(-857.0974f, -1089.754f, 2.162876f),
                        Rotation = new Vector3(-8.01132E-05f, 3.212813E-06f, 24.86326f),
                        Heading = 24.86326f
                    },
                    PedPositions = new List<WPosition>
                    {
                        new WPosition
                        {
                            Position = new Vector3(-861.7628f, -1090.529f, 2.162877f),
                            Rotation = new Vector3(-7.952783E-05f, 2.156467E-06f, 11.34489f),
                            Heading = 11.34489f
                        },
                        new WPosition
                        {
                            Position = new Vector3(-864.7281f, -1094.256f, 2.163f),
                            Rotation = new Vector3(-7.952709E-05f, 2.166828E-06f, -30.08913f),
                            Heading = 329.9109f
                        },
                        new WPosition
                        {
                            Position = new Vector3(-862.457f, -1094.86f, 2.163f),
                            Rotation = new Vector3(-7.952806E-05f, 2.174057E-06f, -27.98836f),
                            Heading = 332.0116f
                        }
                    }
                },
                
                // Bay city avenue
                new MissionWithVanPosition
                {
                    VehiclePosition = new WPosition
                    {
                        Position = new Vector3(-1169.403f, -1772.228f, 3.869389f),
                        Rotation = new Vector3(0f, 0f, -55.93684f),
                        Heading = 304.0632f
                    },
                    PedPositions = new List<WPosition>
                    {
                        new WPosition
                        {
                            Position = new Vector3(-1172.362f, -1779.364f, 3.908163f),
                            Rotation = new Vector3(0f, 0f, -41.90586f),
                            Heading = 318.0941f
                        },
                        new WPosition
                        {
                            Position = new Vector3(-1177.615f, -1780.499f, 3.908465f),
                            Rotation = new Vector3(0f, 0f, -55.35742f),
                            Heading = 304.6426f
                        },
                        new WPosition
                        {
                            Position = new Vector3(-1181.108f, -1774.811f, 3.908466f),
                            Rotation = new Vector3(0f, 0f, -66.97275f),
                            Heading = 293.0273f
                        }
                    }
                },
                
                // Little Seoul
                new MissionWithVanPosition
                {
                    VehiclePosition = new WPosition
                    {
                        Position = new Vector3(-616.1159f, -783.4374f, 25.12997f),
                        Rotation = new Vector3(0f, 0f, -12.30859f),
                        Heading = 347.6914f
                    },
                    PedPositions = new List<WPosition>
                    {
                        new WPosition
                        {
                            Position = new Vector3(-609.6948f, -778.6781f, 25.05415f),
                            Rotation = new Vector3(0f, 0f, 83.44904f),
                            Heading = 83.44904f
                        },
                        new WPosition
                        {
                            Position = new Vector3(-609.1064f, -774.5104f, 25.09307f),
                            Rotation = new Vector3(0f, 0f, 135.8173f),
                            Heading = 135.8173f
                        },
                        new WPosition
                        {
                            Position = new Vector3(-609.1104f, -783.3165f, 25.0321f),
                            Rotation = new Vector3(0f, 0f, 35.94813f),
                            Heading = 35.94814f
                        }
                    }
                },
                
                // Rockford hills
                new MissionWithVanPosition
                {
                    VehiclePosition = new WPosition
                    {
                        Position = new Vector3(-1140.827f, -354.8393f, 37.67485f),
                        Rotation = new Vector3(0f, 0f, -10.97587f),
                        Heading = 349.0241f
                    },
                    PedPositions = new List<WPosition>
                    {
                        new WPosition
                        {
                            Position = new Vector3(-1145.596f, -354.3349f, 37.82365f),
                            Rotation = new Vector3(0f, 0f, -32.40831f),
                            Heading = 327.5917f
                        },
                        new WPosition
                        {
                            Position = new Vector3(-1145.648f, -340.8232f, 37.82365f),
                            Rotation = new Vector3(0f, 0f, -80.16206f),
                            Heading = 279.838f
                        },
                        new WPosition
                        {
                            Position = new Vector3(-1137.295f, -365.7561f, 37.82382f),
                            Rotation = new Vector3(0f, 0f, 13.6225f),
                            Heading = 13.6225f
                        }
                    }
                },
                
                // Pacific Bluffs
                new MissionWithVanPosition
                {
                    VehiclePosition = new WPosition
                    {
                        Position = new Vector3(-1804.24f, -350.9272f, 49.13223f),
                        Rotation = new Vector3(0f, 0f, 26.63698f),
                        Heading = 26.63698f
                    },
                    PedPositions = new List<WPosition>
                    {
                        new WPosition
                        {
                            Position = new Vector3(-1807.783f, -350.6983f, 49.14813f),
                            Rotation = new Vector3(0f, 0f, -19.72607f),
                            Heading = 340.2739f
                        },
                        new WPosition
                        {
                            Position = new Vector3(-1802.878f, -357.7564f, 49.19448f),
                            Rotation = new Vector3(0f, 0f, -9.971686f),
                            Heading = 350.0283f
                        },
                        new WPosition
                        {
                            Position = new Vector3(-1805.372f, -357.7415f, 49.22982f),
                            Rotation = new Vector3(0f, 0f, 1.601322f),
                            Heading = 1.601322f
                        }
                    }
                },
                
                // Vinewood West
                new MissionWithVanPosition
                {
                    VehiclePosition = new WPosition
                    {
                        Position = new Vector3(-637.3102f, 407.927f, 101.2205f),
                        Rotation = new Vector3(0f, 0f, -86.41068f),
                        Heading = 273.5893f
                    },
                    PedPositions = new List<WPosition>
                    {
                        new WPosition
                        {
                            Position = new Vector3(-635.4222f, 400.7935f, 101.1381f),
                            Rotation = new Vector3(0f, 0f, -57.08348f),
                            Heading = 302.9165f
                        },
                        new WPosition
                        {
                            Position = new Vector3(-640.4891f, 415.511f, 101.2802f),
                            Rotation = new Vector3(0f, 0f, -115.3604f),
                            Heading = 244.6396f
                        },
                        new WPosition
                        {
                            Position = new Vector3(-644.5282f, 409.3786f, 101.2092f),
                            Rotation = new Vector3(0f, 0f, -97.10477f),
                            Heading = 262.8952f
                        }
                    }
                },
                
                // Fort Zancudo
                new MissionWithVanPosition
                {
                    VehiclePosition = new WPosition
                    {
                        Position = new Vector3(-1608.419f, 2975.795f, 32.49631f),
                        Rotation = new Vector3(0f, 0f, -100.0347f),
                        Heading = 259.9653f
                    },
                    PedPositions = new List<WPosition>
                    {
                        new WPosition
                        {
                            Position = new Vector3(-1611.167f, 2966.981f, 32.96322f),
                            Rotation = new Vector3(0f, 0f, -53.28904f),
                            Heading = 306.711f
                        },
                        new WPosition
                        {
                            Position = new Vector3(-1618.984f, 2968.175f, 32.92686f),
                            Rotation = new Vector3(0f, 0f, -86.08567f),
                            Heading = 273.9143f
                        },
                        new WPosition
                        {
                            Position = new Vector3(-1607.08f, 2985.182f, 33.21974f),
                            Rotation = new Vector3(0f, 0f, -147.3501f),
                            Heading = 212.6499f
                        }
                    }
                },
                
                // Paleto Forest
                new MissionWithVanPosition
                {
                    VehiclePosition = new WPosition
                    {
                        Position = new Vector3(-743.3853f, 5536.341f, 33.48572f),
                        Rotation = new Vector3(0f, 0f, 25.88297f),
                        Heading = 25.88297f
                    },
                    PedPositions = new List<WPosition>
                    {
                        new WPosition
                        {
                            Position = new Vector3(-740.9667f, 5543.255f, 33.60592f),
                            Rotation = new Vector3(0f, 0f, 94.59106f),
                            Heading = 94.59106f
                        },
                        new WPosition
                        {
                            Position = new Vector3(-748.4689f, 5552.496f, 33.60598f),
                            Rotation = new Vector3(0f, 0f, 107.3971f),
                            Heading = 107.3971f
                        },
                        new WPosition
                        {
                            Position = new Vector3(-748.8306f, 5528.382f, 33.60598f),
                            Rotation = new Vector3(0f, 0f, 31.83799f),
                            Heading = 31.83799f
                        }
                    }
                },
                
                
                
                
            };

        #endregion

        #region AlonePedPossiblePositions

        private static int _lastGivenAlonePedPosition = -1;
        public static WPosition GetRandomAlonePedPosition()
        {
            var pedPositions = MissionWithVanPositions
                .SelectMany(mission => mission.PedPositions)
                .ToList();
            
            var index = RandomHelper.Next(0, pedPositions.Count);
            while (_lastGivenAlonePedPosition == index)
            {
                index = new Random().Next(0, pedPositions.Count);
            }
            
            _lastGivenAlonePedPosition = index;
            return pedPositions[index]; 
        }

        #endregion
        
        #region GuardPosition

        public class GuardPositions
        {
            // List of position for a unique guard
            public List<WPosition> PositionList;
            
            // Actual position of the guard
            public int ActualPosition = 0;
        }
        
        #endregion
    }
}