using GTA;
using GTA.Native;
using Waldhari.Common.Behavior.Property;
using Waldhari.Common.Exceptions;

namespace Waldhari.Common.Misc
{
    public static class PlayerHelper
    {
        
        public static Property.Owner GetCharacterId()
        {
            switch ((PedHash)Game.Player.Character.Model.Hash)
            {
                case PedHash.Michael: return Property.Owner.Michael;
                case PedHash.Trevor: return Property.Owner.Trevor;
                case PedHash.Franklin: return Property.Owner.Franklin;
                default: throw new TechnicalException("Unknown character");
            }
            
        }

        public static bool IsBeingArrested()
        {
            // see https://docs.fivem.net/natives/?_0x388A47C51ABDAC8E
            return Function.Call<bool>(Hash.IS_PLAYER_BEING_ARRESTED, Game.Player, 0);
        }
    }
}