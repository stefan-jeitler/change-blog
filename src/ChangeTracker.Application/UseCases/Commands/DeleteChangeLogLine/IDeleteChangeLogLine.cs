﻿using System.Threading.Tasks;

namespace ChangeTracker.Application.UseCases.Commands.DeleteChangeLogLine
{
    public interface IDeleteChangeLogLine
    {
        Task ExecuteAsync(IDeleteChangeLogLineOutputPort output, DeleteChangeLogLineRequestModel requestModel);
    }
}