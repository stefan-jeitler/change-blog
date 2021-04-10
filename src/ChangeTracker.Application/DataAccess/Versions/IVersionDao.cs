using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ChangeTracker.Domain.ChangeLog;
using ChangeTracker.Domain.Version;
using CSharpFunctionalExtensions;

namespace ChangeTracker.Application.DataAccess.Versions
{
    public interface IVersionDao
    {
        Task<Maybe<ClVersionInfo>> FindAsync(Guid projectId, ClVersion version);
        Task<Result<ClVersionInfo, Conflict>> AddAsync(ClVersionInfo versionInfo);
    }
}