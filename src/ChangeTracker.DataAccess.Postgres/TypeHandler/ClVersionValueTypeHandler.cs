﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChangeTracker.Domain.Version;
using Dapper;

namespace ChangeTracker.DataAccess.Postgres.TypeHandler
{
    public class ClVersionValueTypeHandler : SqlMapper.TypeHandler<ClVersionValue>
    {
        public override void SetValue(IDbDataParameter parameter, ClVersionValue value)
        {
            parameter.Value = value.Value;
        }

        public override ClVersionValue Parse(object value) => ClVersionValue.Parse(value.ToString());
    }
}
