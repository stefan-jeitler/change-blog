using System;
using System.Threading.Tasks;
using ChangeTracker.Domain.ChangeLog;
using OneOf;

namespace ChangeTracker.Application.DataAccess.Versions
{
    public interface IChangeLogDao
    {
        Task<OneOf<ChangeLogLine, Conflict>> AddChangeLogLineAsync(ChangeLogLine changeLogLine);
        Task<ChangeLogInfo> GetChangeLogInfoAsync(Guid projectId, Guid? versionId);
    }
}