using System;
using System.Threading.Tasks;

namespace ChangeBlog.Application.UseCases.Versions.DeleteVersion;

public interface IDeleteVersion
{
    Task ExecuteAsync(IDeleteVersionOutputPort output, Guid versionId);
}