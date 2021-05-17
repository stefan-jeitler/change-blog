using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChangeTracker.Application.DataAccess;
using ChangeTracker.Application.DataAccess.Projects;
using ChangeTracker.Application.UseCases;
using ChangeTracker.Domain;
using ChangeTracker.Domain.Common;
using CSharpFunctionalExtensions;
using Dapper;
using Microsoft.Extensions.Logging;
using static ChangeTracker.DataAccess.Postgres.DataAccessObjects.ProjectDaoSqlStatements;

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
            var project = await _dbAccessor.DbConnection
                .QueryFirstOrDefaultAsync<Project>(FindProjectByAccountAndNameSql, new
                {
                    accountId,
                    name = name.Value.ToLower()
                });

            return project == default
                ? Maybe<Project>.None
                : Maybe<Project>.From(project);
        }

        public async Task<Maybe<Project>> FindProjectAsync(Guid projectId)
        {
            var project = await _dbAccessor.DbConnection
                .QueryFirstOrDefaultAsync<Project>(FindProjectByProjectIdSql, new
                {
                    projectId
                });

            return project == default
                ? Maybe<Project>.None
                : Maybe<Project>.From(project);
        }

        public async Task<Project> GetProjectAsync(Guid projectId)
        {
            var project = await FindProjectAsync(projectId);

            if (project.HasNoValue)
                throw new Exception(
                    "The requested project does not exist. If you are not sure whether the project exists use 'FindProject' otherwise file an issue.");

            return project.Value;
        }

        public async Task<IList<Project>> GetProjectsAsync(ProjectQuerySettings querySettings)
        {
            var sql = GetProjectsForAccountSql(querySettings.LastProjectId.HasValue,
                querySettings.IncludeClosedProjects);

            var projects = await _dbAccessor.DbConnection
                .QueryAsync<Project>(sql, new
                {
                    querySettings.AccountId,
                    querySettings.UserId,
                    permission = Permission.ViewAccountProjects.ToString(),
                    querySettings.LastProjectId,
                    count = (int) querySettings.Count
                });

            return projects.ToList();
        }

        public async Task<Result<Project, Conflict>> AddProjectAsync(Project newProject)
        {
            const string insertProjectSql = @"
                    INSERT INTO project (id, account_id, versioning_scheme_id, name, created_by_user, closed_at, created_at)
                    VALUES (@id, @accountId, @versioningSchemeId, @name, @user, @closedAt, @createdAt)";

            try
            {
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

        public async Task CloseProjectAsync(Project project)
        {
            if (!project.ClosedAt.HasValue)
                throw new Exception("The given project has no closed date.");

            const string closeProjectSql = "UPDATE project SET closed_at = @closedAt WHERE id = @projectId";

            await _dbAccessor.DbConnection.ExecuteScalarAsync(closeProjectSql, new
            {
                closedAt = project.ClosedAt,
                projectId = project.Id
            });
        }
    }
}