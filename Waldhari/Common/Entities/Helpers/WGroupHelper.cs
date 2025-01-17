using System.Collections.Generic;
using GTA;
using Waldhari.Common.Files;
using Waldhari.Common.Misc;

namespace Waldhari.Common.Entities.Helpers
{
    public static class WGroupHelper
    {
        public class Gang
        {
            internal List<PedHash> PedHashes;
            internal List<VehicleHash> VehicleHashes;

            private int _lastGivenPedHash = -1;
            public PedHash GetRandomPedHash()
            {
                var index = RandomHelper.Next(0, PedHashes.Count);
                while (index == _lastGivenPedHash)
                {
                    index = RandomHelper.Next(0, PedHashes.Count);
                }
                _lastGivenPedHash = index;
                return PedHashes[index];
            }
            
            private int _lastGivenVehicleHash = -1;
            public VehicleHash GetRandomVehicleHash()
            {
                var index = RandomHelper.Next(0, VehicleHashes.Count);
                while (index == _lastGivenVehicleHash)
                {
                    index = RandomHelper.Next(0, VehicleHashes.Count);
                }
                _lastGivenVehicleHash = index;
                return VehicleHashes[index];
            }
        }
        
        public static readonly List<Gang> Gangs = new List<Gang>
        {
            // The LOST
            new Gang
            {
                PedHashes = new List<PedHash>
                {
                    PedHash.Lost01GMY,
                    PedHash.Lost02GMY,
                    PedHash.Lost03GMY,
                    PedHash.GenBiker01AMM
                },
                VehicleHashes = new List<VehicleHash>
                {
                    VehicleHash.ZombieA,
                    VehicleHash.ZombieB,
                    VehicleHash.Nightblade,
                    VehicleHash.RatBike,
                    VehicleHash.Wolfsbane,
                    VehicleHash.Daemon,
                    VehicleHash.Daemon2,
                    VehicleHash.Esskey,
                    VehicleHash.Hexer,
                    VehicleHash.GBurrito2,
                    VehicleHash.SlamVan2,
                }
            },
            
            // Hill Billies
            new Gang()
            {
                PedHashes = new List<PedHash>
                {
                    PedHash.Hillbilly01AMM,
                    PedHash.Hillbilly02AMM,
                    PedHash.Taphillbilly,
                    PedHash.Rurmeth01AMM,
                    PedHash.Acult02AMO,
                    PedHash.Trucker01SMM,
                    PedHash.ExArmy01,
                    PedHash.Methhead01AMY
                },
                VehicleHashes = new List<VehicleHash>
                {
                    VehicleHash.BfInjection,
                    VehicleHash.Rebel,
                    VehicleHash.Rebel2,
                    VehicleHash.Blazer,
                    VehicleHash.Blazer2,
                    VehicleHash.Blazer3,
                    VehicleHash.Blazer4,
                    VehicleHash.Blazer5,
                    VehicleHash.Riata,
                    VehicleHash.Sandking,
                    VehicleHash.Sandking2
                }
            },
            
            // The BALLAS
            new Gang
            {
                PedHashes = new List<PedHash>
                {
                    PedHash.BallasLeader,
                    PedHash.BallaEast01GMY,
                    PedHash.BallaOrig01GMY,
                    PedHash.BallaSout01GMY
                },
                VehicleHashes = new List<VehicleHash>
                {
                    VehicleHash.Peyote,
                    VehicleHash.Peyote2,
                    VehicleHash.Peyote3,
                    VehicleHash.Peyote3,
                    VehicleHash.Tornado,
                    VehicleHash.Tornado2,
                    VehicleHash.Tornado3,
                    VehicleHash.Tornado4,
                    VehicleHash.Tornado5,
                    VehicleHash.Tornado6,
                    VehicleHash.Baller,
                    VehicleHash.Baller2,
                    VehicleHash.Baller3,
                    VehicleHash.Baller4,
                    VehicleHash.Baller5,
                    VehicleHash.Baller6,
                    VehicleHash.Baller7
                }
            },
            
            // Mexicans
            new Gang
            {
                PedHashes = new List<PedHash>
                {
                    PedHash.MexBoss01GMM,
                    PedHash.MexBoss02GMM,
                    PedHash.MexCntry01AMM,
                    PedHash.MexGang01GMY,
                    PedHash.MexGoon01GMY,
                    PedHash.MexGoon02GMY,
                    PedHash.MexGoon03GMY,
                    PedHash.MexThug01AMY,
                    PedHash.MexLabor01AMM
                },
                VehicleHashes = new List<VehicleHash>
                {
                    VehicleHash.Cavalcade,
                    VehicleHash.Cavalcade2,
                    VehicleHash.Peyote,
                    VehicleHash.Peyote2,
                    VehicleHash.Peyote3,
                    VehicleHash.Peyote3,
                    VehicleHash.RancherXL,
                    VehicleHash.RancherXL2,
                    VehicleHash.Rebel,
                    VehicleHash.Rebel2,
                }
            }
            
        };

        private static int _lastGivenGang = -1;
        public static Gang GetRandomGang()
        {
            var index = RandomHelper.Next(0, Gangs.Count);
            while (index == _lastGivenGang)
            {
                index = RandomHelper.Next(0, Gangs.Count);
            }
            _lastGivenGang = index;
            return Gangs[index];
        }
        
        public static WGroup CreateRivalMembers(int number, Gang gang = null, bool withVehicles = true)
        {
            if(gang == null) gang = GetRandomGang();
            
            var group = new WGroup();
            group.Name = "RivalMembers";
            group.Create(Relationship.Hate);

            for (var i = 0; i < number; i++)
            {
                Logger.Info("Create rival member "+i);
                
                var wPed = new WPed
                {
                    PedHash = gang.GetRandomPedHash(),
                    WBlip = WBlipHelper.GetEnemy("rival_enemy")
                };
                
                // add ped to list, then, in script,
                // when peds are created, add there to the group (see concerned) script
                group.WPeds.Add(wPed);
                
                Logger.Info("Rival member added "+i);
            }

            var total = ((int)(number / 2)) + 1;
            for (var i = 0; i < total; i++)
            {
                if (!withVehicles) continue;
                
                var wVehicle = new WVehicle
                {
                    VehicleHash = gang.GetRandomVehicleHash()
                };
                group.AddWVehicle(wVehicle);
                
                Logger.Info("Vehicle added "+i);
            }
            
            return group;
        }
        
        
        
    }
}