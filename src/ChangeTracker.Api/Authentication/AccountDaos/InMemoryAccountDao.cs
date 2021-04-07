using System;
using System.Threading.Tasks;

namespace ChangeTracker.Api.Authentication.AccountDaos
{
    public class InMemoryAccountDao : IAccountDao
    {
        private readonly string _apiKey;
        public readonly Guid TestAccountId = Guid.Parse("3e7ff9f5-954a-4953-8fe2-618f6b4a6e11");

        public InMemoryAccountDao(string apiKey)
        {
            _apiKey = apiKey;
        }

        public Task<Guid?> FindAccountId(string apiKey) =>
            _apiKey.Equals(apiKey, StringComparison.OrdinalIgnoreCase)
                ? Task.FromResult((Guid?) TestAccountId)
                : Task.FromResult((Guid?) null);
    }
}