using System.Linq;
using System.Threading.Tasks;
using ChangeTracker.Application.DataAccess.Accounts;
using ChangeTracker.Application.Tests.TestDoubles;
using ChangeTracker.Application.UseCases.Queries.GetRoles;
using FluentAssertions;
using Moq;
using Xunit;

namespace ChangeTracker.Application.Tests.UseCaseTests.Queries.GetRoles
{
    public class GetRolesInteractorTests
    {
        private readonly Mock<IRolesDao> _rolesDaoMock;

        public GetRolesInteractorTests()
        {
            _rolesDaoMock = new Mock<IRolesDao>();
        }

        private GetRolesInteractor CreateInteractor()
        {
            return new(_rolesDaoMock.Object);
        }

        [Fact]
        public async Task GetRoles_HappyPath_Successfully()
        {
            // arrange
            _rolesDaoMock.Setup(m => m.GetRolesAsync()).ReturnsAsync(TestRoles.Roles);
            var interactor = CreateInteractor();

            // act
            var roles = await interactor.ExecuteAsync(null);

            // assert
            roles.Should().HaveCount(TestRoles.Roles.Count);
        }
        
        [Fact]
        public async Task GetRoles_FilterDeveloperRole_Successfully()
        {
            // arrange
            _rolesDaoMock.Setup(m => m.GetRolesAsync()).ReturnsAsync(TestRoles.Roles);
            var interactor = CreateInteractor();

            // act
            var roles = await interactor.ExecuteAsync("developer");

            // assert
            roles.Should().HaveCount(1);
        }
        
        [Fact]
        public async Task GetRoles_FilterDeveloperRole_ResultProperlyPopulated()
        {
            // arrange
            _rolesDaoMock.Setup(m => m.GetRolesAsync()).ReturnsAsync(TestRoles.Roles);
            var interactor = CreateInteractor();

            // act
            var roles = await interactor.ExecuteAsync("developer");

            // assert
            var developerRole = roles.Single();
            developerRole.Name.Should().Be("Developer");
            developerRole.Permissions.Should().HaveCount(4);
            developerRole.Permissions.Should().Contain("ViewOwnUser");
            developerRole.Permissions.Should().Contain("ViewAccount");
            developerRole.Permissions.Should().Contain("AddProduct");
            developerRole.Permissions.Should().Contain("ViewChangeLogLines");
        }
    }
}