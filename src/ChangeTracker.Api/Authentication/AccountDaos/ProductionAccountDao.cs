using System;
using System.Threading.Tasks;

namespace ChangeTracker.Api.Authentication.AccountDaos
{
    public class ProductionAccountDao : IAccountDao
    {
        public Task<Guid?> FindAccountId(string apiKey) => throw new NotImplementedException();
    }
}