using System;
using ChangeBlog.Application.UseCases.SharedModels;

namespace ChangeBlog.Application.UseCases.Versions.GetLatestVersion;

public interface IGetLatestVersionOutputPort
{
    void VersionFound(VersionResponseModel versionResponseModel);

    void NoVersionExists(Guid productId);
    void ProductDoesNotExist();
}