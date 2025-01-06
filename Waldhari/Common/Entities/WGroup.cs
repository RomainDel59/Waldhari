using System.Collections.Generic;
using System.Linq;
using GTA;
using GTA.Native;
using Waldhari.Common.Exceptions;

namespace Waldhari.Common.Entities
{
    public class WGroup
    {
        public string Name = null;

        // If not ally : enemy
        public bool IsAlly = false;

        public List<WPed> WPeds = null;
        public List<WVehicle> WVehicles = null;

        public bool AreDead() => WPeds == null || WPeds.All(wPed => wPed == null || wPed.Ped == null || wPed.Ped.IsDead);
        public bool HasVehicles() => WVehicles != null && WVehicles.Count > 0;

        private RelationshipGroup _relationshipGroup = null;
        private PedGroup _pedGroup = null;
        
        /// <summary>
        /// Creates a group of ped allied to each other.
        /// If IsAlly is true, all peds added to the group will be allied to the player,
        /// otherwise there will be enemies.
        /// </summary>
        /// <exception cref="TechnicalException">If the name is empty</exception>
        public void Create()
        {
            if(Name == null) throw new TechnicalException("Name cannot be empty");

            int num;
            unsafe
            {
                Function.Call(Hash.ADD_RELATIONSHIP_GROUP, Name, &num);
            }
            _relationshipGroup = new RelationshipGroup(num);
            
            // All peds in this group will be allies to each other
            _relationshipGroup.SetRelationshipBetweenGroups(_relationshipGroup, Relationship.Companion);

            if (IsAlly)
            {
                // All peds in this group will be allies with the player
                _relationshipGroup.SetRelationshipBetweenGroups(Game.Player.Character.RelationshipGroup, Relationship.Companion, true);
            }
            else
            {
                // All peds in this group will be enemies with the player
                _relationshipGroup.SetRelationshipBetweenGroups(Game.Player.Character.RelationshipGroup, Relationship.Hate, true);
            }

            _pedGroup = new PedGroup();
            _pedGroup.Formation = Formation.Loose;
            
            WPeds = new List<WPed>();
            
            WVehicles = new List<WVehicle>();
        }

        /// <summary>
        /// Removes all peds, group and relationship. It deletes everything, not only mark as no longer needed. 
        /// If something has already been removed, does nothing.
        /// Nothing is preserved.
        /// </summary>
        public void Remove()
        {
            if(WPeds != null)
            {
                foreach (var wPed in WPeds)
                {
                    wPed?.Remove();
                }

                WPeds?.Clear();
                WPeds = null;
            }

            if (WVehicles != null)
            {
                foreach (var wVehicle in WVehicles)
                {
                    wVehicle?.Remove();
                }
                
                WVehicles?.Clear();
                WVehicles = null;
            }
            
            _pedGroup?.Dispose();
            _pedGroup = null;
            
            _relationshipGroup = null;
        }

        /// <summary>
        /// Adds a ped to the group.
        /// </summary>
        /// <param name="wPed">Ped to add</param>
        /// <param name="leader">The ped is the leader</param>
        public void AddWPed(WPed wPed, bool leader = false)
        {
            WPeds.Add(wPed);
            _pedGroup.Add(wPed.Ped, leader);
            wPed.Ped.RelationshipGroup = _relationshipGroup;
            wPed.Ped.NeverLeavesGroup = true;
        }

        /// <summary>
        /// Adds a vehicle to the group, so the peds can use it.
        /// </summary>
        /// <param name="wVehicle"></param>
        public void AddWVehicle(WVehicle wVehicle)
        {
            WVehicles.Add(wVehicle);
        }









        // public void Update(bool forceDelete = false)
        // {
        //     foreach (var wPed in WPedList)
        //     {
        //         if (wPed == null) continue;
        //         if (wPed.GtaPed == null) continue;
        //
        //         wPed.AttachEnemyMarker();
        //         
        //         if(DisappearanceDistance == -1) throw new TechnicalException("DisappearanceDistance is not defined.");
        //
        //         if (forceDelete || wPed.GtaPed.Position.DistanceTo(Game.Player.Character.Position) > DisappearanceDistance)
        //         {
        //             wPed.Remove();
        //             continue;
        //         }
        //
        //         if (!wPed.GtaPed.IsDead) continue;
        //
        //         wPed.RemoveBlip();
        //         wPed.MarkAsNoLongerNeeded();
        //     }
        // }
    }
}