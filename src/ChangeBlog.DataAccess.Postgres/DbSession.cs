using System.Data;
using ChangeBlog.Application.DataAccess;

namespace ChangeBlog.DataAccess.Postgres
{
    public sealed class DbSession : IDbAccessor, IUnitOfWork
    {
        private readonly LazyDbConnection _dbConnection;
        private uint _startedUows;
        private IDbTransaction _transaction;

        public DbSession(LazyDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public IDbConnection DbConnection => _dbConnection.Value;

        public void Start()
        {
            if (_startedUows == 0)
            {
                BeginTransaction();
            }

            _startedUows++;
        }

        public void Commit()
        {
            if (_startedUows == 1)
            {
                CommitTransaction();
            }

            if (_startedUows > 0)
                _startedUows--;
        }

        private void CommitTransaction()
        {
            _transaction.Commit();
            _transaction.Dispose();
            _dbConnection.Value.Close();
        }

        private void BeginTransaction()
        {
            if (_dbConnection.Value.State != ConnectionState.Open)
                _dbConnection.Value.Open();

            _transaction = _dbConnection.Value.BeginTransaction(IsolationLevel.RepeatableRead);
        }
    }
}
