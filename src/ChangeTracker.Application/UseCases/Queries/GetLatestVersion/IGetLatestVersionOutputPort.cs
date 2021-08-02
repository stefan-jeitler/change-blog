using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChangeTracker.Application.UseCases.Queries.SharedModels;

namespace ChangeTracker.Application.UseCases.Queries.GetLatestVersion
{
    public interface IGetLatestVersionOutputPort
    {
        void VersionFound(VersionResponseModel versionResponseModel);

        void NoVersionExists(Guid productId);
        void ProductDoesNotExist();
    }
}
