using System;

namespace Waldhari.Common.Exceptions
{
    public class TechnicalException : Exception
    {
        public TechnicalException(string message) : base(message)
        {
        }
    }
}