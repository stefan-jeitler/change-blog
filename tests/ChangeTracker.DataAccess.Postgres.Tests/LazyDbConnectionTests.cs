using System.Data;
using Moq;
using Xunit;

namespace ChangeTracker.DataAccess.Postgres.Tests
{
    public class LazyDbConnectionTests
    {
        [Fact]
        public void DbConnection_NotInstantiated_NotDisposed()
        {
            var dbConnection = new Mock<IDbConnection>();
            using (new LazyDbConnection(() => dbConnection.Object))
            {
            }

            dbConnection.Verify(x => x.Dispose(), Times.Never);
        }

        [Fact]
        public void DbConnection_Instantiated_Disposed()
        {
            var dbConnection = new Mock<IDbConnection>();
            using (var lazyDbConnection = new LazyDbConnection(() => dbConnection.Object))
            {
                _ = lazyDbConnection.Value;
            }

            dbConnection.Verify(x => x.Dispose(), Times.Once);
        }
    }
}