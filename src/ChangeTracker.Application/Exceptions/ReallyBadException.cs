using System;

namespace ChangeTracker.Application.Exceptions
{
    [Serializable]
    public class ReallyBadException : Exception
    {
        public ReallyBadException(string message) : base(message)
        {
        }
    }
}