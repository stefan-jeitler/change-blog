﻿using System.Threading.Tasks;

namespace ChangeTracker.Application.UseCases.Commands.UpdateChangeLogLine
{
    public interface IUpdateChangeLogLine
    {
        Task ExecuteAsync(IUpdateLineOutputPort output, ChangeLogLineRequestModel requestModel);
    }
}