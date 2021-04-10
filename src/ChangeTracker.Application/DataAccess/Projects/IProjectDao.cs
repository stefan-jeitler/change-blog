﻿using System;
using System.Threading.Tasks;
using ChangeTracker.Domain;
using ChangeTracker.Domain.Common;
using CSharpFunctionalExtensions;

namespace ChangeTracker.Application.DataAccess.Projects
{
    public interface IProjectDao
    {
        Task<Maybe<Project>> FindAsync(Guid accountId, Name name);
        Task<Maybe<Project>> FindAsync(Guid projectId);
        Task<Result<Project, Conflict>> AddAsync(Project newProject);
    }
}