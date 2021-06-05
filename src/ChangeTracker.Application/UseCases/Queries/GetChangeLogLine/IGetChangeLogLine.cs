using System;
using System.Threading.Tasks;
using ChangeTracker.Application.UseCases.Queries.SharedModels;
using CSharpFunctionalExtensions;

namespace ChangeTracker.Application.UseCases.Queries.GetChangeLogLine
{
    public interface IGetChangeLogLine
    {
        Task<Maybe<ChangeLogLineResponseModel>> ExecuteAsync(Guid userId, Guid changeLogLineId);
    }
}