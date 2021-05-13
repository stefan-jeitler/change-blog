using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChangeTracker.Application.DataAccess.Projects;
using ChangeTracker.Application.DataAccess.Users;
using ChangeTracker.Application.Extensions;
using ChangeTracker.Domain;

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

        public async Task<IEnumerable<ProjectsQueryResponseModel>> ExecuteAsync(
            ProjectsQueryRequestModel queryRequestModel)
        {
            var projects = queryRequestModel.AccountId.HasValue
                ? await _projectDao.GetAccountProjectsAsync(queryRequestModel.AccountId.Value,
                    queryRequestModel.Count,
                    queryRequestModel.LastProjectId)
                : await _projectDao.GetUserProjects(queryRequestModel.UserId, queryRequestModel.Count,
                    queryRequestModel.LastProjectId);

            var currentUser = await _userDao.GetUserAsync(queryRequestModel.UserId);

            var userIds = projects
                .Select(x => x.CreatedByUser)
                .Distinct()
                .ToList();

            var users = await _userDao.GetUsersAsync(userIds);
            var userById = users.ToDictionary(x => x.Id, x => x);

            return projects
                .Select(x => CreateResponse(x, userById, currentUser.TimeZone));
        }

        private static ProjectsQueryResponseModel CreateResponse(Project project,
            IReadOnlyDictionary<Guid, User> userById,
            string timeZone)
        {
            var user = userById[project.CreatedByUser];
            var userName = user.Email;
            var createdAtLocal = project.CreatedAt.ToLocal(timeZone);

            return new ProjectsQueryResponseModel(project.Id, project.AccountId, project.Name.Value,
                project.VersioningScheme.Id,
                project.VersioningScheme.Name, userName, createdAtLocal);
        }
    }
}