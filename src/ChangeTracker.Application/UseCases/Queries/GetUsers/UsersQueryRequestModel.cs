using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChangeTracker.Application.UseCases.Queries.GetUsers
{
    public class UsersQueryRequestModel
    {
        public const ushort MaxChunkCount = 200;

        public UsersQueryRequestModel(Guid userId, Guid accountId, Guid? lastUserId = null, ushort count = MaxChunkCount)
        {
            if (accountId == Guid.Empty)
                throw new ArgumentException("AccountId cannot be empty.");

            AccountId = accountId;

            if (userId == Guid.Empty)
                throw new ArgumentException("UserId cannot be empty.");

            UserId = userId;

            Count = Math.Min(MaxChunkCount, count);
            LastUserId = lastUserId;
        }

        public Guid AccountId { get; }
        public Guid UserId { get; }
        public ushort Count { get; }
        public Guid? LastUserId { get; }
    }
}
