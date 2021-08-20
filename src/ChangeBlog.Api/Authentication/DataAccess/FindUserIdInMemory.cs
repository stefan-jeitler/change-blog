using System;
using System.Threading.Tasks;

namespace ChangeBlog.Api.Authentication.DataAccess
{
    public class FindUserIdInMemory : IFindUserId
    {
        private readonly string _apiKey;
        public readonly Guid UserId = Guid.Parse("3e7ff9f5-954a-4953-8fe2-618f6b4a6e11");

        public FindUserIdInMemory(string apiKey)
        {
            _apiKey = apiKey;
        }

        public Task<Guid?> FindAsync(string apiKey) =>
            _apiKey.Equals(apiKey, StringComparison.Ordinal)
                ? Task.FromResult((Guid?) UserId)
                : Task.FromResult((Guid?) null);
    }
}