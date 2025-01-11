using System;
using System.Collections.Generic;
using System.Linq;
using GTA;
using GTA.Native;
using Waldhari.Common.Exceptions;
using Waldhari.Common.Files;

namespace Waldhari.Common.Entities
{
    public class WGroup
    {
        public string Name = null;

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
        public void Create(Relationship relationship)
        {
            if(Name == null) throw new TechnicalException("Name cannot be empty");

            CreateRelationship(relationship);
            
            _pedGroup = new PedGroup();
            _pedGroup.Formation = Formation.Loose;
            
            WPeds = new List<WPed>();
            
            WVehicles = new List<WVehicle>();
        }

        private void CreateRelationship(Relationship relationship)
        {
            if (_relationshipGroup != null)
            {
                Logger.Debug($"Before create relationship, as group {Name} has already been created, it will remove relationship");
                RemoveRelationship();
            }
            
            if(Name == null) Name = Guid.NewGuid().ToString();
            
            int num;
            unsafe
            {
                Function.Call(Hash.ADD_RELATIONSHIP_GROUP, Name, &num);
            }
            _relationshipGroup = new RelationshipGroup(num);
            
            // All peds in this group will be allies to each other
            _relationshipGroup.SetRelationshipBetweenGroups(_relationshipGroup, Relationship.Companion);
            
            // All peds in this group will be in relationship with the player
            _relationshipGroup.SetRelationshipBetweenGroups(Game.Player.Character.RelationshipGroup, relationship, true);

            Logger.Debug($"After create relationship : " +
                         $"group internal relationship={_relationshipGroup.GetRelationshipBetweenGroups(_relationshipGroup)}, " +
                         $"relationship with player={_relationshipGroup.GetRelationshipBetweenGroups(Game.Player.Character.RelationshipGroup)}");
            
            if (WPeds != null && WPeds.Count > 0)
            {
                foreach (var wPed in WPeds)
                {
                    if(wPed?.Ped == null) continue;
                    
                    wPed.Ped.RelationshipGroup = _relationshipGroup;
                }
            }
            
        }

        private void RemoveRelationship()
        {
            Logger.Debug($"Before delete relationship : " +
                         $"group internal relationship={_relationshipGroup.GetRelationshipBetweenGroups(_relationshipGroup)}, " +
                         $"relationship with player={_relationshipGroup.GetRelationshipBetweenGroups(Game.Player.Character.RelationshipGroup)}");
                
            _relationshipGroup.ClearRelationshipBetweenGroups(_relationshipGroup, _relationshipGroup.GetRelationshipBetweenGroups(_relationshipGroup));
            _relationshipGroup.ClearRelationshipBetweenGroups(Game.Player.Character.RelationshipGroup, _relationshipGroup.GetRelationshipBetweenGroups(Game.Player.Character.RelationshipGroup), true);
            _relationshipGroup.Remove();
            _relationshipGroup = null;
            
            Logger.Debug($"After delete relationship : " +
                         $"group internal relationship={_relationshipGroup.GetRelationshipBetweenGroups(_relationshipGroup)}, " +
                         $"relationship with player={_relationshipGroup.GetRelationshipBetweenGroups(Game.Player.Character.RelationshipGroup)}");
        }

        public void MakeNeutral()
        {
            // All peds in this group will be neutral with the player
            CreateRelationship(Relationship.Pedestrians);
            //see Relationship class : Pedestrians=The correct relationship name for this enum would be None.

            if (WPeds != null && WPeds.Count > 0)
            {
                foreach (var wPed in WPeds)
                {
                    if(wPed?.Ped == null) continue;
                    
                    wPed.Ped.Task.ClearAll();
                }
            }
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
        /// Adds a ped in the relationship group.
        /// The ped should have been already added to the list WPeds!
        /// </summary>
        /// <param name="wPed">Ped to add</param>
        /// <param name="leader">The ped is the leader</param>
        public void AddInGroup(WPed wPed, bool leader = false)
        {
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

        public void MarkAsNoLongerNeeded()
        {
            MakeNeutral();
            
            if (WPeds != null)
            {
                foreach (var wPed in WPeds)
                {
                    if (wPed == null) continue;

                    wPed.WBlip?.Remove();
                    wPed.WBlip = null;
                    wPed.Ped?.MarkAsNoLongerNeeded();
                    wPed.Ped = null;
                }
            }

            if (WVehicles != null)
            {
                foreach (var wVehicle in WVehicles)
                {
                    if (wVehicle == null) continue;

                    wVehicle.Vehicle?.MarkAsNoLongerNeeded();
                    wVehicle.Vehicle = null;
                }
            }
        }
        
    }
}