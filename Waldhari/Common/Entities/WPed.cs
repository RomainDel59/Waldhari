using GTA;
using Waldhari.Common.Entities.Helpers;
using Waldhari.Common.Exceptions;
using Waldhari.Common.UI;

namespace Waldhari.Common.Entities
{
    public class WPed
    {
        public Ped Ped;

        public WPosition InitialPosition = null;

        public string Scenario = null;
        
        public string AnimationDictionnary = null;
        public string AnimationName = null;
        
        public PedHash PedHash = default;
        
        public WBlip WBlip = null;

        
        /// <summary>
        /// Creates the ped according entered properties.
        /// If the ped has already been created, does nothing.
        /// If the model cannot be loaded, error will be logged.
        /// Blip is not created if required.
        /// </summary>
        /// <exception cref="TechnicalException">If at least one of required property is empty</exception>
        public void Create()
        {
            if(InitialPosition == null) throw new TechnicalException("InitialPosition cannot be empty");
            if(PedHash == default) throw new TechnicalException("PedHash cannot be empty");

            Model model = PedHash;
            model.Request(1000);
            Ped = World.CreatePed(model, InitialPosition.Position);
            model.MarkAsNoLongerNeeded();

            MoveInPosition();
        }
        
        /// <summary>
        /// Removes the ped. It deletes it, not only mark as no longer needed. 
        /// If the ped has already been removed, does nothing.
        /// Properties of WPed class are preserved,
        /// only the ped is delete and its property set to null.
        /// Do the same to the WBlip if exists.
        /// </summary>
        public void Remove()
        {
            WBlip?.Remove();
            Ped?.Delete();
            Ped = null;
        }
        
        /// <summary>
        /// Moves the ped immediately in the initial position.
        /// </summary>
        public void MoveInPosition()
        {
            if(InitialPosition == null) throw new TechnicalException("InitialPosition cannot be empty");
            if(Ped == null) throw new TechnicalException("Ped cannot be empty");
            
            Ped.PositionNoOffset = InitialPosition.Position;
            Ped.Rotation = InitialPosition.Rotation;
            Ped.Heading = InitialPosition.Heading;
        }

        /// <summary>
        /// Add weapon to the ped with 150 ammo, choosing the best and authorizing to switch if it has multiple.
        /// </summary>
        /// <param name="weaponHash">New weapon to add</param>
        public void AddWeapon(WeaponHash weaponHash)
        {
            Ped.Weapons.Give(weaponHash, 150, true, true);
            Ped.Weapons.Select(Ped.Weapons.BestWeapon);
            Ped.CanSwitchWeapons = true;
        }
        
        public void MakeMissionDestination(string nameKey)
        {
            if(WBlip == null)
            {
                WBlip = WBlipHelper.GetMission(nameKey);
                WBlip.Ped = Ped;
            }
            
            WBlip.Create();
            
            MarkerHelper.DrawEntityMarkerOnBlip(WBlip);
        }

        public void RemoveMissionDestination()
        {
            WBlip?.Remove();
        }
    }
}