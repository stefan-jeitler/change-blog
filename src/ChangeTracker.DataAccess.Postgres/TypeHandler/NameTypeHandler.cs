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
    public class NameTypeHandler : SqlMapper.TypeHandler<Name>
    {
        public override void SetValue(IDbDataParameter parameter, Name value)
        {
            parameter.Value = value.Value;
        }

        public override Name Parse(object value) => Name.Parse(value.ToString());
    }
}
