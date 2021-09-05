using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChangeBlog.Application.DataAccess.ExternalIdentity
{
    public interface IExternalUserInfoDao
    {
        Task<UserInfo> GetAsync();
    }
}
