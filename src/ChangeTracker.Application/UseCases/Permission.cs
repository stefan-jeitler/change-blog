﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChangeTracker.Application.UseCases
{
    public enum Permission
    {
        ViewChangeLogLines,
        OperateChangeLogLines,
        ViewVersions,
        OperateVersions
    }
}
