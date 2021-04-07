using System;
using System.Data;

namespace ChangeTracker.DataAccess.Postgres
{
    public sealed class LazyDbConnection : Lazy<IDbConnection>, IDisposable
    {
        public LazyDbConnection(Func<IDbConnection> valueFactory)
            : base(valueFactory)
        {
        }

        public void Dispose()
        {
            if (!IsValueCreated)
            {
                return;
            }

            Value?.Dispose();
        }
    }
}