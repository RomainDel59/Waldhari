using GTA;
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
        
        public static int GetCharacterId()
        {
            switch ((PedHash)Game.Player.Character.Model.Hash)
            {
                case PedHash.Michael: return 1;
                case PedHash.Trevor: return 2;
                case PedHash.Franklin: return 3;
                default: throw new TechnicalException("Unknown character");
            }
            
        }
    }
}