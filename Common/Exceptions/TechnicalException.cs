using System;

namespace GTAVMods.Utils
{
    public class TechnicalException : Exception
    {
        public TechnicalException(string message) : base(message)
        {
        }
    }
}