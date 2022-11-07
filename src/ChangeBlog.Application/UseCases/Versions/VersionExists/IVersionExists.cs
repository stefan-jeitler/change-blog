using System;
using System.Threading.Tasks;

namespace ChangeBlog.Application.UseCases.Versions.VersionExists;

public interface IVersionExists
{
    Task<bool> ExecuteAsync(Guid versionId);
}