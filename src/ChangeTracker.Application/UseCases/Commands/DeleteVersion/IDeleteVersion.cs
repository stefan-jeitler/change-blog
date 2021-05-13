using System;
using System.Threading.Tasks;

namespace ChangeTracker.Application.UseCases.Commands.DeleteVersion
{
    public interface IDeleteVersion
    {
        Task ExecuteAsync(IDeleteVersionOutputPort output, Guid versionId);
    }
}