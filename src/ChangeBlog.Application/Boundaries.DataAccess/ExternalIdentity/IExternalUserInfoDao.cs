using System.Threading.Tasks;

namespace ChangeBlog.Application.Boundaries.DataAccess.ExternalIdentity;

public interface IExternalUserInfoDao
{
    Task<UserInfo> GetAsync();
}