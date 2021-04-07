using System;
using System.Threading.Tasks;
using ChangeTracker.Domain.Version;
using CSharpFunctionalExtensions;

namespace ChangeTracker.Application.DataAccess.Projects
{
    public interface IVersioningSchemeDao
    {
        Task<Maybe<VersioningScheme>> FindAsync(Guid versioningSchemeId);
    }
}