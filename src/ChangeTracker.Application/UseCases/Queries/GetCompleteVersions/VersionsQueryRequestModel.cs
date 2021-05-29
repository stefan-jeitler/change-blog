using System;

namespace ChangeTracker.Application.UseCases.Queries.GetCompleteVersions
{
    public class VersionsQueryRequestModel
    {
        public const ushort MaxLimit = 100;
        public const ushort MaxSearchTermLength = 50;

        public VersionsQueryRequestModel(Guid productId, Guid? lastVersionId, Guid userId, string searchTerm,
            ushort limit, bool includeDeleted = false)
        {
            if (productId == Guid.Empty)
                throw new ArgumentException("ProductId cannot be empty.");

            ProductId = productId;
            LastVersionId = lastVersionId;

            if (userId == Guid.Empty)
                throw new ArgumentException("UserId cannot be empty.");

            UserId = userId;

            if (searchTerm is not null && searchTerm.Length > MaxSearchTermLength)
                throw new ArgumentException($"SearchTerm is too long. Max length: {MaxSearchTermLength}");

            SearchTerm = searchTerm;
            Limit = Math.Min(limit, MaxLimit);
            IncludeDeleted = includeDeleted;
        }

        public Guid ProductId { get; }
        public Guid? LastVersionId { get; }
        public Guid UserId { get; }
        public string SearchTerm { get; }
        public ushort Limit { get; }
        public bool IncludeDeleted { get; }
    }
}