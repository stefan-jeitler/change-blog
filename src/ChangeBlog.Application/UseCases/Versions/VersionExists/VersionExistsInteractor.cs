using System;
using System.Threading.Tasks;
using ChangeBlog.Application.Boundaries.DataAccess.Versions;

namespace ChangeBlog.Application.UseCases.Versions.VersionExists;

public class VersionExistsInteractor : IVersionExists
{
    private readonly IVersionDao _versionDao;

    public VersionExistsInteractor(IVersionDao versionDao)
    {
        _versionDao = versionDao;
    }

    public async Task<bool> ExecuteAsync(Guid versionId)
    {
        var version = await _versionDao.FindVersionAsync(versionId);

        return version.HasValue;
    }
}