using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ChangeTracker.Domain.Version;
using CSharpFunctionalExtensions;

namespace ChangeTracker.Application.DataAccess.Products
{
    public interface IVersioningSchemeDao
    {
        Task<Maybe<VersioningScheme>> FindSchemeAsync(Guid versioningSchemeId);
        Task<VersioningScheme> GetSchemeAsync(Guid versioningSchemeId);
        Task<IList<VersioningScheme>> GetSchemesAsync(IList<Guid> versioningSchemeIds);
    }
}