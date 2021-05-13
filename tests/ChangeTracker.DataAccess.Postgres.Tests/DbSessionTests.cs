using System.Data;
using FluentAssertions;
using Moq;
using Xunit;

namespace ChangeTracker.DataAccess.Postgres.Tests
{
    public class DbSessionTests
    {
        [Fact]
        public void DbSession_StartUow_DbConnectionOpened()
        {
            // arrange
            var dbConnectionMock = new Mock<IDbConnection>();
            var dbTransactionMock = new Mock<IDbTransaction>();

            dbConnectionMock.Setup(x => x.Open());
            dbConnectionMock.Setup(x => x.BeginTransaction()).Returns(dbTransactionMock.Object);

            var lazyDbConnection = new LazyDbConnection(() => dbConnectionMock.Object);
            var dbSession = new DbSession(lazyDbConnection);

            // act
            dbSession.Start();

            // assert
            dbConnectionMock.Verify(x => x.Open(), Times.Once);
        }

        [Fact]
        public void DbSession_StartTwoUows_OnlyOneConnectionOpened()
        {
            // arrange
            var dbConnectionMock = new Mock<IDbConnection>();
            var dbTransactionMock = new Mock<IDbTransaction>();

            dbConnectionMock.Setup(x => x.BeginTransaction()).Returns(dbTransactionMock.Object);

            var lazyDbConnection = new LazyDbConnection(() => dbConnectionMock.Object);
            var dbSession = new DbSession(lazyDbConnection);

            // act
            dbSession.Start();
            dbSession.Start();

            // assert
            dbConnectionMock.Verify(x => x.Open(), Times.Once);
        }

        [Fact]
        public void DbSession_StartTwoUowAndCommitBoth_SecondCallCommits()
        {
            // arrange
            var dbConnectionMock = new Mock<IDbConnection>();
            var dbTransactionMock = new Mock<IDbTransaction>();

            dbConnectionMock.Setup(x => x.BeginTransaction(It.IsAny<IsolationLevel>()))
                .Returns(dbTransactionMock.Object);

            var lazyDbConnection = new LazyDbConnection(() => dbConnectionMock.Object);
            var dbSession = new DbSession(lazyDbConnection);

            dbSession.Start();
            dbSession.Start();

            // act
            dbSession.Commit();
            dbSession.Commit();

            // assert
            dbTransactionMock.Verify(m => m.Commit(), Times.Once);
        }

        [Fact]
        public void DbSession_DbConnectionIsAvailableIfNoUowStarted_Successful()
        {
            // arrange
            var dbConnectionMock = new Mock<IDbConnection>();
            var lazyDbConnection = new LazyDbConnection(() => dbConnectionMock.Object);
            var dbSession = new DbSession(lazyDbConnection);

            // act
            var dbConnection = dbSession.DbConnection;

            // assert
            dbConnection.Should().Be(dbConnectionMock.Object);
        }

        [Fact]
        public void DbSession_CommitTwoTimesWithoutStartedUow_NothingHappens()
        {
            // arrange
            var dbConnectionMock = new Mock<IDbConnection>();
            var dbTransactionMock = new Mock<IDbTransaction>();

            dbConnectionMock.Setup(x => x.BeginTransaction()).Returns(dbTransactionMock.Object);

            var lazyDbConnection = new LazyDbConnection(() => dbConnectionMock.Object);
            var dbSession = new DbSession(lazyDbConnection);


            // act
            dbSession.Commit();
            dbSession.Commit();

            // assert
            dbTransactionMock.Verify(m => m.Commit(), Times.Never);
        }
    }
}