using System;
using ChangeTracker.Domain.Common;

namespace ChangeTracker.Application.UseCases.Commands.AddVersion
{
    public interface IAddVersionOutputPort
    {
        void Created(Guid versionId);
        void InvalidVersionFormat(string version);
        void VersionDoesNotMatchScheme(string version, string versioningSchemeName);
        void ProductDoesNotExist(Guid productId);
        void Conflict(string reason);
        void VersionAlreadyExists(Guid versionId);
        void ProductClosed(Guid productId);
        void InvalidVersionName(string name);
    }
}