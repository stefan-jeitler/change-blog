using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Npgsql;
using Xunit;

namespace ChangeTracker.DataAccess.Postgres.Tests
{
    public class UserAccessDaoTests
    {
        private readonly UserAccessDao _testUserAccessDao;

        public UserAccessDaoTests()
        {
            _testUserAccessDao = new UserAccessDao(() => new NpgsqlConnection(Configuration.ConnectionString));
        }

        [Fact]
        public async Task XY()
        {
            
        }
    }
}
