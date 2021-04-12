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
        Task<Maybe<ClVersion>> FindAsync(Guid projectId, ClVersionValue versionValue);
        Task<Maybe<ClVersion>> FindAsync(Guid projectId, Guid versionId);
        Task<Result<ClVersion, Conflict>> AddAsync(ClVersion version);
    }
}