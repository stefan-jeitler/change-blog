using System;
using System.Threading.Tasks;

namespace ChangeTracker.Api.Authentication
{
    public interface IFindUserId
    {
        Task<Guid?> FindAsync(string apiKey);
    }
}