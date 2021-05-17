using System;
using System.Threading.Tasks;
using ChangeTracker.Application.DataAccess.Projects;
using ChangeTracker.DataAccess.Postgres.DataAccessObjects;
using ChangeTracker.Domain;
using ChangeTracker.Domain.Common;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Npgsql;
using Xunit;

// ReSharper disable InconsistentNaming

namespace ChangeTracker.DataAccess.Postgres.Tests.DataAccessObjectsTests
{
    public class ProjectDaoTests : IDisposable
    {
        private readonly LazyDbConnection _lazyDbConnection;

        public ProjectDaoTests()
        {
            _lazyDbConnection = new LazyDbConnection(() => new NpgsqlConnection(Configuration.ConnectionString));
        }

        public void Dispose()
        {
            _lazyDbConnection?.Dispose();
        }

        private ProjectDao CreateDao()
        {
            return new(new DbSession(_lazyDbConnection), NullLogger<ProjectDao>.Instance);
        }

        [Fact]
        public async Task FindProject_ByAccountIdAndName_ReturnsProject()
        {
            var projectDao = CreateDao();
            var t_ua_account_01_proj_02 = Guid.Parse("0614f8d6-8895-4c74-bcbe-8a3c26076e1b");
            var t_ua_account_01 = Guid.Parse("ec3a44cc-0ba4-4c97-ad7f-911e9f6a73bc");

            var project =
                await projectDao.FindProjectAsync(t_ua_account_01, Name.Parse(nameof(t_ua_account_01_proj_02)));

            project.HasValue.Should().BeTrue();
            project.Value.Id.Should().Be(t_ua_account_01_proj_02);
        }

        [Fact]
        public async Task FindProject_ByProjectId_ReturnsProject()
        {
            var projectDao = CreateDao();
            var t_ua_account_01_proj_02 = Guid.Parse("0614f8d6-8895-4c74-bcbe-8a3c26076e1b");

            var project =
                await projectDao.FindProjectAsync(t_ua_account_01_proj_02);

            project.HasValue.Should().BeTrue();
            project.Value.Id.Should().Be(t_ua_account_01_proj_02);
        }

        [Fact]
        public async Task GetProject_ExistingProject_ReturnsProject()
        {
            var projectDao = CreateDao();
            var t_ua_account_01_proj_02 = Guid.Parse("0614f8d6-8895-4c74-bcbe-8a3c26076e1b");

            var project = await projectDao.GetProjectAsync(t_ua_account_01_proj_02);

            project.Id.Should().Be(t_ua_account_01_proj_02);
        }

        [Fact]
        public void GetProject_NotExistingProject_Exception()
        {
            var projectDao = CreateDao();
            var notExistingProjectId = Guid.Parse("21f05095-c016-4f60-b98a-03c037b6cc8c");

            Func<Task<Project>> act = () => projectDao.GetProjectAsync(notExistingProjectId);

            act.Should().ThrowExactly<Exception>();
        }

        [Fact]
        public async Task GetProjects_HappyPath_ReturnsProjects()
        {
            var projectDao = CreateDao();
            var t_ua_account_01 = Guid.Parse("ec3a44cc-0ba4-4c97-ad7f-911e9f6a73bc");
            var t_ua_account_01_user_02 = Guid.Parse("7aa9004b-ed6f-4862-8307-579030c860be");
            var querySettings = new ProjectQuerySettings(t_ua_account_01, t_ua_account_01_user_02);

            var projects = await projectDao.GetProjectsAsync(querySettings);

            projects.Should().HaveCount(2);
        }

        [Fact]
        public async Task GetProjects_LimitResultBy1_ReturnsOnlyOneProject()
        {
            var projectDao = CreateDao();
            var t_ua_account_01 = Guid.Parse("ec3a44cc-0ba4-4c97-ad7f-911e9f6a73bc");
            var t_ua_account_01_user_02 = Guid.Parse("7aa9004b-ed6f-4862-8307-579030c860be");
            var querySettings = new ProjectQuerySettings(t_ua_account_01, t_ua_account_01_user_02, null, 1);

            var projects = await projectDao.GetProjectsAsync(querySettings);

            projects.Should().HaveCount(1);
        }

        [Fact]
        public async Task GetProjects_SkipFirstProject_ReturnsSecond()
        {
            var projectDao = CreateDao();
            var t_ua_account_01 = Guid.Parse("ec3a44cc-0ba4-4c97-ad7f-911e9f6a73bc");
            var t_ua_account_01_user_02 = Guid.Parse("7aa9004b-ed6f-4862-8307-579030c860be");
            var lastProjectId = Guid.Parse("139a2e54-e9be-4168-98b4-2839d9b3db04");
            var querySettings = new ProjectQuerySettings(t_ua_account_01, t_ua_account_01_user_02, lastProjectId);

            var projects = await projectDao.GetProjectsAsync(querySettings);

            projects.Should().HaveCount(1);
            projects.Should().Contain(x => x.Id == Guid.Parse("0614f8d6-8895-4c74-bcbe-8a3c26076e1b"));
        }
    }
}