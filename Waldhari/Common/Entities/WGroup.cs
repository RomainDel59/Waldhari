using System.Collections.Generic;
using GTA;
using GTA.Native;
using Waldhari.Common.Exceptions;

namespace Waldhari.Common.Entities
{
    public class WGroup
    {
        public static int DisappearanceDistance = -1;
        
        public PedGroup PedGroup;
        public List<WPed> WPedList;
        public int Relationship;

        public void Create(string name)
        {
            int relationshipTemp;
            unsafe
            {
                Function.Call(Hash.ADD_RELATIONSHIP_GROUP, name, &relationshipTemp);
            }

            Relationship = relationshipTemp;

            Function.Call(Hash.SET_RELATIONSHIP_BETWEEN_GROUPS, GTA.Relationship.Hate, Relationship, Game.Player.Character.RelationshipGroup);
            Function.Call(Hash.SET_RELATIONSHIP_BETWEEN_GROUPS, GTA.Relationship.Hate, Game.Player.Character.RelationshipGroup, Relationship);
            Function.Call(Hash.SET_RELATIONSHIP_BETWEEN_GROUPS, GTA.Relationship.Companion, Relationship, Relationship);

            PedGroup = new PedGroup();
            PedGroup.Formation = Formation.Loose;
        }

        public void Add(WPed ped, bool leader = false)
        {
            if (WPedList == null) WPedList = new List<WPed>();

            ped.GtaPed.RelationshipGroup = Relationship;

            PedGroup.Add(ped.GtaPed, leader);

            WPedList.Add(ped);
        }

        public void AttachEnemyMarker()
        {
            foreach (var wPed in WPedList)
            {
                wPed?.AttachEnemyMarker();
            }
        }

        public bool IsDead()
        {
            var isDead = true;
            foreach (var wPed in WPedList)
            {
                if (wPed != null && wPed.GtaPed != null && !wPed.GtaPed.IsDead)
                {
                    isDead = false;
                    break;
                }
            }

            return isDead;
        }

        public void Update(bool forceDelete = false)
        {
            foreach (var wPed in WPedList)
            {
                if (wPed == null) continue;
                if (wPed.GtaPed == null) continue;

                wPed.AttachEnemyMarker();
                
                if(DisappearanceDistance == -1) throw new TechnicalException("DisappearanceDistance is not defined.");

                if (forceDelete || wPed.GtaPed.Position.DistanceTo(Game.Player.Character.Position) > DisappearanceDistance)
                {
                    wPed.Remove();
                    continue;
                }

                if (!wPed.GtaPed.IsDead) continue;

                wPed.RemoveBlip();
                wPed.MarkAsNoLongerNeeded();
            }
        }
    }
}