using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChangeTracker.Api.Authorization.RequestBodIdentifiers
{
    public interface IContainsAccountId
    {
        Guid AccountId { get; }
    }
}
