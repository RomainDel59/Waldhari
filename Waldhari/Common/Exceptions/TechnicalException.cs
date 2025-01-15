using System;
using Waldhari.Common.Files;

namespace Waldhari.Common.Exceptions
{
    public class TechnicalException : Exception
    {
        public TechnicalException(string message) : base(message)
        {
            Logger.Exception(message);
        }
    }
}