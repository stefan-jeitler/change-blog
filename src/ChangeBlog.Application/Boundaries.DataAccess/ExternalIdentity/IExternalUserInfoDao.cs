using System.Threading.Tasks;
using CSharpFunctionalExtensions;

namespace ChangeBlog.Application.Boundaries.DataAccess.ExternalIdentity;

public interface IExternalUserInfoDao
{
    Task<UserInfo> GetUserInfoAsync();
    Task<Maybe<UserPhoto>> GetUserPhotoAsync();
}