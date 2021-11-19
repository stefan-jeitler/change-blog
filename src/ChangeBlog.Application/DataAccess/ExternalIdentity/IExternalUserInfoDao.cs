using System.Threading.Tasks;

namespace ChangeBlog.Application.DataAccess.ExternalIdentity;

public interface IExternalUserInfoDao
{
    Task<UserInfo> GetAsync();
}