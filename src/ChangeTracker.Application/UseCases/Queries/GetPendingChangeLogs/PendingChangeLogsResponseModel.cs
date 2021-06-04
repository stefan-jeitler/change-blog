using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChangeTracker.Application.UseCases.Queries.SharedModels;

namespace ChangeTracker.Application.UseCases.Queries.GetPendingChangeLogs
{
    public class PendingChangeLogsResponseModel
    {
        public PendingChangeLogsResponseModel(Guid productId, string productName, Guid accountId, List<ChangeLogLineResponseModel> changeLogs)
        {
            if (productId == Guid.Empty)
                throw new ArgumentException("ProductId cannot be empty.");

            ProductId = productId;
            ProductName = productName ?? throw new ArgumentNullException(nameof(productName));

            if (accountId == Guid.Empty)
                throw new ArgumentException("AccountId cannot be empty.");

            AccountId = accountId;

            ChangeLogs = changeLogs ?? throw new ArgumentNullException(nameof(changeLogs));
        }

        public Guid ProductId { get; }
        public string ProductName { get; }
        public Guid AccountId { get; }
        public List<ChangeLogLineResponseModel> ChangeLogs { get; }
    }
}
