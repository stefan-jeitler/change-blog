﻿using System;
using System.Threading.Tasks;
using ChangeTracker.Application.DataAccess;
using ChangeTracker.Application.DataAccess.Projects;
using ChangeTracker.Domain;
using ChangeTracker.Domain.Common;
using CSharpFunctionalExtensions;
using Dapper;
using Microsoft.Extensions.Logging;

namespace ChangeTracker.DataAccess.Postgres.DataAccessObjects
{
    public class ProjectDao : IProjectDao
    {
        private readonly IDbAccessor _dbAccessor;
        private readonly ILogger<ProjectDao> _logger;

        public ProjectDao(IDbAccessor dbAccessor, ILogger<ProjectDao> logger)
        {
            _dbAccessor = dbAccessor;
            _logger = logger;
        }

        public async Task<Maybe<Project>> FindProjectAsync(Guid accountId, Name name)
        {
            const string findProjectSql = @"
                SELECT p.id,
                       p.account_id AS accountId,
                       p.name,
                       vs.id AS versioningSchemeId,
                       vs.name AS versioningSchemeName,
                       vs.regex_pattern AS regexPattern,
                       vs.description,
                       vs.created_at AS versioningSchemeCreatedAt,
                       vs.deleted_at AS versioningSchemeDeletedAt,
                       p.created_by_user AS createdByUser,
                       p.created_at AS createdAt,
                       p.closed_at AS closedAt
                FROM project p
                JOIN account a on p.account_id = a.id
                JOIN versioning_scheme vs on p.versioning_scheme_id = vs.id
                WHERE a.id = @accountId
                AND LOWER(p.name) = @name";

            var project = await _dbAccessor.DbConnection
                .QueryFirstOrDefaultAsync<Project>(findProjectSql, new
                {
                    accountId,
                    name = name.Value.ToLower()
                });

            return project == default
                ? Maybe<Project>.None
                : Maybe<Project>.From(project);
        }

        public Task<Maybe<Project>> FindProjectAsync(Guid projectId) => throw new NotImplementedException();

        public Task<Project> GetProjectAsync(Guid projectId) => throw new NotImplementedException();

        public async Task<Result<Project, Conflict>> AddProjectAsync(Project newProject)
        {
            try
            {
                const string insertProjectSql = @"
                    INSERT INTO project (id, account_id, versioning_scheme_id, name, created_by_user, closed_at, created_at)
                    VALUES (@id, @accountId, @versioningSchemeId, @name, @user, @closedAt, @createdAt)";

                await _dbAccessor.DbConnection
                    .ExecuteAsync(insertProjectSql, new
                    {
                        id = newProject.Id,
                        accountId = newProject.AccountId,
                        versioningSchemeId = newProject.VersioningScheme.Id,
                        name = newProject.Name,
                        user = newProject.CreatedByUser,
                        closedAt = newProject.ClosedAt,
                        createdAt = newProject.CreatedAt
                    });

                return Result.Success<Project, Conflict>(newProject);
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
                throw;
            }
        }

        public Task<Result<Project, Conflict>> CloseProjectAsync(Project project) =>
            throw new NotImplementedException();
    }
}