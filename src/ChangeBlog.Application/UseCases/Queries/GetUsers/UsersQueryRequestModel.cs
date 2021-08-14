using System;

namespace ChangeBlog.Application.UseCases.Queries.GetUsers
{
    public class UsersQueryRequestModel
    {
        public const ushort MaxLimit = 200;

        public UsersQueryRequestModel(Guid userId, Guid accountId, Guid? lastUserId = null,
            ushort limit = MaxLimit)
        {
            if (accountId == Guid.Empty)
                throw new ArgumentException("AccountId cannot be empty.");

            AccountId = accountId;

            if (userId == Guid.Empty)
                throw new ArgumentException("UserId cannot be empty.");

            UserId = userId;

            Limit = Math.Min(MaxLimit, limit);
            LastUserId = lastUserId;
        }

        public Guid AccountId { get; }
        public Guid UserId { get; }
        public ushort Limit { get; }
        public Guid? LastUserId { get; }
    }
}
