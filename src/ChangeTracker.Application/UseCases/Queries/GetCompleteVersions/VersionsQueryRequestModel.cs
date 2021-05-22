using System;

namespace ChangeTracker.Application.UseCases.Queries.GetCompleteVersions
{
    public class VersionsQueryRequestModel
    {
        public const ushort MaxLimit = 100;

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