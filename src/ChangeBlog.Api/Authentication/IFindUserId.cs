using System;
using System.Threading.Tasks;

namespace ChangeBlog.Api.Authentication
{
    public interface IFindUserId
    {
        Task<Guid?> FindAsync(string apiKey);
    }
}
