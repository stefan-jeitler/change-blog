using System;
using System.Collections.Generic;
using ChangeTracker.Application.UseCases.Queries.SharedModels;

namespace ChangeTracker.Application.UseCases.Queries.GetCompleteVersions
{
    public class CompleteVersionResponseModel
    {
        public CompleteVersionResponseModel(Guid versionId, Guid productId, string productName, Guid accountId,
            DateTime createdAt, List<ChangeLogLineResponseModel> changeLogs)
        {
            if (versionId == Guid.Empty)
                throw new ArgumentException("VersionId cannot be empty.");
            
            VersionId = versionId;
            
            if (productId == Guid.Empty)
                throw new ArgumentException("ProductId cannot be empty.");
            
            ProductId = productId;
            ProductName = productName ?? throw new ArgumentNullException(nameof(productName));
            
            if (accountId == Guid.Empty)
                throw new ArgumentException("AccountId cannot be empty.");
            
            AccountId = accountId;

            if (createdAt == DateTime.MinValue || createdAt == DateTime.MaxValue)
                throw new ArgumentException("Invalid creation date.");
            
            CreatedAt = createdAt;
            ChangeLogs = changeLogs ?? throw new ArgumentNullException(nameof(changeLogs));
        }

        public Guid VersionId { get; }
        public Guid ProductId { get; }
        public string ProductName { get; }
        public Guid AccountId { get; }
        public DateTime CreatedAt { get; }
        public List<ChangeLogLineResponseModel> ChangeLogs { get;}
        
    }
}