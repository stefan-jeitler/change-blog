using System.Data;
using ChangeTracker.Application.DataAccess;

namespace ChangeTracker.DataAccess.Postgres
{
    public sealed class DbSession : IDbAccessor, IUnitOfWork
    {
        private readonly LazyDbConnection _dbConnection;
        private readonly object _lock = new();
        private uint _startedUows;
        private IDbTransaction _transaction;

        public DbSession(LazyDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public IDbConnection DbConnection => _dbConnection.Value;

        public void Start()
        {
            lock (_lock)
            {
                if (_startedUows >= 1)
                {
                    return;
                }

                if (_dbConnection.Value.State != ConnectionState.Open)
                {
                    _dbConnection.Value.Open();
                }

                _transaction = _dbConnection.Value.BeginTransaction();
                _startedUows++;
            }
        }

        public void Commit()
        {
            lock (_lock)
            {
                if (_startedUows == 1)
                {
                    _transaction.Commit();
                    _transaction.Dispose();
                    _dbConnection.Value.Close();
                }

                if (_startedUows > 0)
                    _startedUows--;
            }
        }
    }
}