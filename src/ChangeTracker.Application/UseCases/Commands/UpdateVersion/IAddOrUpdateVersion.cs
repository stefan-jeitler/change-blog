﻿using System.Threading.Tasks;
using ChangeTracker.Application.UseCases.Commands.SharedModels;

namespace ChangeTracker.Application.UseCases.Commands.UpdateVersion
{
    public interface IAddOrUpdateVersion
    {
        Task ExecuteAsync(IAddOrUpdateVersionOutputPort output, VersionRequestModel requestModel);
    }
}