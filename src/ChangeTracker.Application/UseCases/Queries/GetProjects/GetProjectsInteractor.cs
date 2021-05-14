using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChangeTracker.Application.DataAccess.Projects;
using ChangeTracker.Application.DataAccess.Users;
using ChangeTracker.Application.Extensions;
using ChangeTracker.Domain;
using CSharpFunctionalExtensions;

namespace ChangeTracker.Application.UseCases.Queries.GetProjects
{
    public class GetProjectsInteractor : IGetProjects
    {
        private readonly IProjectDao _projectDao;
        private readonly IUserDao _userDao;

        public GetProjectsInteractor(IProjectDao projectDao, IUserDao userDao)
        {
            _projectDao = projectDao;
            _userDao = userDao;
        }

        public async Task<IEnumerable<ProjectResponseModel>> ExecuteAsync(
            ProjectsQueryRequestModel requestModel)
        {
            var projects = await _projectDao.GetProjectsAsync(requestModel.AccountId,
                requestModel.UserId,
                requestModel.Count,
                requestModel.LastProjectId);

            var currentUser = await _userDao.GetUserAsync(requestModel.UserId);

            var userIds = projects
                .Select(x => x.CreatedByUser)
                .Distinct()
                .ToList();

            var users = await _userDao.GetUsersAsync(userIds);
            var userById = users.ToDictionary(x => x.Id, x => x);

            return projects
                .Select(x => CreateResponse(x, userById, currentUser.TimeZone));
        }

        public async Task<Maybe<ProjectResponseModel>> ExecuteAsync(Guid userId, Guid projectId)
        {
            var project = await _projectDao.FindProjectAsync(projectId);
            var currentUser = await _userDao.GetUserAsync(userId);

            return await project
                .Map(async p => new {User = await _userDao.GetUserAsync(p.CreatedByUser), Project = p})
                .Map(x => CreateResponse(x.Project, x.User, currentUser.TimeZone));
        }

        private static ProjectResponseModel CreateResponse(Project project,
            IReadOnlyDictionary<Guid, User> userById,
            string timeZone)
        {
            var user = userById[project.CreatedByUser];
            return CreateResponse(project, user, timeZone);
        }

        private static ProjectResponseModel CreateResponse(Project project, User user,
            string timeZone)
        {
            var userName = user.Email;
            var createdAtLocal = project.CreatedAt.ToLocal(timeZone);

            return new ProjectResponseModel(project.Id, 
                project.AccountId,
                project.Name.Value,
                project.VersioningScheme.Id,
                project.VersioningScheme.Name, 
                userName,
                createdAtLocal,
                project.ClosedAt);
        }
    }
}