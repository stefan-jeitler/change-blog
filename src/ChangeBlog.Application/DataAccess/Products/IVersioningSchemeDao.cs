using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ChangeBlog.Domain.Version;
using CSharpFunctionalExtensions;

namespace ChangeBlog.Application.DataAccess.Products;

public interface IVersioningSchemeDao
{
    Task<Maybe<VersioningScheme>> FindSchemeAsync(Guid versioningSchemeId);
    Task<VersioningScheme> GetSchemeAsync(Guid versioningSchemeId);
    Task<IList<VersioningScheme>> GetSchemesAsync(IList<Guid> versioningSchemeIds);
}