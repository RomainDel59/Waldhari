using GTA;
using Waldhari.Common.Misc;

namespace Waldhari.Common.Entities.Helpers
{
    public static class WeaponsHelper
    {
        private static readonly WeaponHash[] GangWeapons =
        {
            WeaponHash.Pistol,
            WeaponHash.CombatPistol,
            WeaponHash.HeavyPistol,
            WeaponHash.SNSPistol,
            WeaponHash.SMG,
            WeaponHash.MicroSMG,
            WeaponHash.MachinePistol,
            WeaponHash.PumpShotgun,
            WeaponHash.SawnOffShotgun,
            WeaponHash.AssaultRifle,
            WeaponHash.CompactRifle,
            WeaponHash.SpecialCarbine,
            WeaponHash.Bat,
            WeaponHash.Crowbar,
            WeaponHash.Knife,
            WeaponHash.Machete
        };

        public static WeaponHash GetRandomGangWeapon()
        {
            var index = RandomHelper.Next(0, GangWeapons.Length);
            return GangWeapons[index];
        }
    }
}