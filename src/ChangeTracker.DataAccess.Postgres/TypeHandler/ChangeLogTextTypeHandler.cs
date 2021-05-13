using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChangeTracker.Domain.ChangeLog;
using Dapper;

namespace ChangeTracker.DataAccess.Postgres.TypeHandler
{
    public class ChangeLogTextTypeHandler : SqlMapper.TypeHandler<ChangeLogText>
    {
        public override void SetValue(IDbDataParameter parameter, ChangeLogText value)
        {
            parameter.Value = value.Value;
        }

        public override ChangeLogText Parse(object value) => ChangeLogText.Parse(value.ToString());
    }
}
