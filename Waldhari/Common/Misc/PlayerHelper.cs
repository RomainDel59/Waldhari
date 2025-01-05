using GTA;
using Waldhari.Behavior.Property;
using Waldhari.Common.Exceptions;

namespace Waldhari.Common.Misc
{
    public static class PlayerHelper
    {
        public static string GetCharacterName()
        {
            switch ((PedHash)Game.Player.Character.Model.Hash)
            {
                case PedHash.Michael: return "Michael";
                case PedHash.Trevor: return "Trevor";
                case PedHash.Franklin: return "Franklin";
                default: throw new TechnicalException("Unknown character");
            }
            
        }
        
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
    }
}