using System;

namespace ChangeTracker.Application.DataAccess
{
    public record Conflict
    {
        public Conflict(string reason)
        {
            Reason = reason ?? throw new ArgumentNullException(nameof(reason));
        }

        public string Reason { get; }

        public static implicit operator string(Conflict conflict) => conflict.Reason;
    }
}