using System;
using System.Threading.Tasks;

namespace ChangeTracker.Api.Authentication
{
    public interface IAccountDao
    {
        Task<Guid?> FindAccountId(string apiKey);
    }
}