using System;
using ChangeBlog.Application.UseCases.Queries.SharedModels;

namespace ChangeBlog.Application.UseCases.Queries.GetLatestVersion;

public interface IGetLatestVersionOutputPort
{
    void VersionFound(VersionResponseModel versionResponseModel);

    void NoVersionExists(Guid productId);
    void ProductDoesNotExist();
}