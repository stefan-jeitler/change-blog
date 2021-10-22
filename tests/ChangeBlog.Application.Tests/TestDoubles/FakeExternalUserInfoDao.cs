using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChangeBlog.Application.DataAccess.ExternalIdentity;
using ChangeBlog.Domain;

namespace ChangeBlog.Application.Tests.TestDoubles
{
    public class FakeExternalUserInfoDao : IExternalUserInfoDao
    {

        public UserInfo UserInfo { get; set; }

        public Task<UserInfo> GetAsync() => Task.FromResult(UserInfo);
    }
}
