using System;
using System.Threading.Tasks;
using ChangeTracker.Domain.ChangeLogVersion;
using CSharpFunctionalExtensions;
using OneOf;

namespace ChangeTracker.Application.DataAccess.Versions
{
    public interface IVersionDao
    {
        Task<Maybe<ClVersionInfo>> FindAsync(Guid projectId, ClVersion version);
        Task<OneOf<ClVersionInfo, Conflict>> AddAsync(ClVersionInfo clVersion);
    }
}