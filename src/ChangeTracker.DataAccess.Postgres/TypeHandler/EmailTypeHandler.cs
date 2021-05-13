using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChangeTracker.Domain.Common;
using Dapper;

namespace ChangeTracker.DataAccess.Postgres.TypeHandler
{
    public class EmailTypeHandler : SqlMapper.TypeHandler<Email>
    {
        public override void SetValue(IDbDataParameter parameter, Email value)
        {
            parameter.Value = value.Value;
        }

        public override Email Parse(object value) => Email.Parse(value.ToString());
    }
}
