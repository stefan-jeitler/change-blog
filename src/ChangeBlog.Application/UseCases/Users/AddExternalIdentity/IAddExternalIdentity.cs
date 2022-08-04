using System;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;

namespace ChangeBlog.Application.UseCases.Users.AddExternalIdentity;

public interface IAddExternalIdentity
{
    Task<Result<Guid>> ExecuteAsync(string externalUserId);
}