using System;
using System.Threading.Tasks;
using ChangeBlog.Application.DataAccess.ExternalIdentity;

namespace ChangeBlog.DataAccess.MicrosoftIdentity
{
    public class ExternalUserInfoNullObject : IExternalUserInfoDao
    {
        public Task<UserInfo> GetAsync() =>
            throw new NotSupportedException("External UserInfo is not yet supported in this api.");
    }
}