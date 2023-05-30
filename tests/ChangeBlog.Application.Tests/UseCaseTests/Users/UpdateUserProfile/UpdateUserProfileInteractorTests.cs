using System;
using System.Linq;
using System.Threading.Tasks;
using ChangeBlog.Application.Boundaries.DataAccess;
using ChangeBlog.Application.Tests.TestDoubles;
using ChangeBlog.Application.UseCases.Users.UpdateUserProfile;
using ChangeBlog.Domain;
using ChangeBlog.Domain.Miscellaneous;
using FluentAssertions;
using Moq;
using Xunit;

namespace ChangeBlog.Application.Tests.UseCaseTests.Users.UpdateUserProfile;

public class UpdateUserProfileInteractorTests
{
    private readonly FakeUserDao _fakeUserDao;
    private readonly Mock<IUpdateUserProfileOutputPort> _outputPortMock;
    private readonly Mock<IBusinessTransaction> _unitOfWorkMock;

    public UpdateUserProfileInteractorTests()
    {
        _fakeUserDao = new FakeUserDao();
        _unitOfWorkMock = new Mock<IBusinessTransaction>();
        _outputPortMock = new Mock<IUpdateUserProfileOutputPort>(MockBehavior.Strict);
    }

    private IUpdateUserProfile CreateInteractor()
    {
        return new UpdateUserProfileInteractor(_fakeUserDao, _unitOfWorkMock.Object);
    }

    [Fact]
    public async Task UpdateUserProfile_UserIdIsEmpty_ArgumentException()
    {
        await Task.Yield();

        // arrange
        var updateProfileRequestModel = new UpdateUserProfileRequestModel(Guid.Empty, "Europe/Berlin", "de-AT");
        var interactor = CreateInteractor();

        // act
        var act = () => interactor.ExecuteAsync(_outputPortMock.Object, updateProfileRequestModel);

        // assert
        await act.Should().ThrowExactlyAsync<ArgumentException>();
    }

    [Fact]
    public async Task UpdateUserProfile_NoValuesGiven_NothingChanged()
    {
        await Task.Yield();

        // arrange
        var currentTimezone = Name.Parse("Europe/Berlin");
        var currentCulture = Name.Parse("de-AT");
        var testUser = new User(TestAccount.User.Id, TestAccount.User.Email, TestAccount.User.FirstName,
            TestAccount.User.LastName,
            currentTimezone, currentCulture, null, DateTime.Parse("2022-05-01"));

        _outputPortMock.Setup(m => m.Updated(It.IsAny<Guid>()));

        _fakeUserDao.Users.Add(testUser);
        var updateProfileRequestModel = new UpdateUserProfileRequestModel(testUser.Id, null, null);
        var interactor = CreateInteractor();

        // act
        await interactor.ExecuteAsync(_outputPortMock.Object, updateProfileRequestModel);

        // assert
        _outputPortMock.Verify(m => m.Updated(It.Is<Guid>(x => x == testUser.Id)), Times.Once);
    }

    [Fact]
    public async Task UpdateUserProfile_OnlyTimezoneIsGiven_TimezoneUpdate()
    {
        // arrange
        var currentTimezone = Name.Parse("Europe/Berlin");
        var currentCulture = Name.Parse("de-AT");
        var testUser = new User(TestAccount.User.Id, TestAccount.User.Email, TestAccount.User.FirstName,
            TestAccount.User.LastName,
            currentTimezone, currentCulture, null, DateTime.Parse("2022-05-01"));

        const string newTimezone = "Europe/Vienna";

        _outputPortMock.Setup(m => m.Updated(It.IsAny<Guid>()));

        _fakeUserDao.Users.Add(testUser);
        var updateProfileRequestModel = new UpdateUserProfileRequestModel(testUser.Id, newTimezone, null);
        var interactor = CreateInteractor();

        // act
        await interactor.ExecuteAsync(_outputPortMock.Object, updateProfileRequestModel);

        // assert
        _outputPortMock.Verify(m => m.Updated(It.Is<Guid>(x => x == testUser.Id)), Times.Once);
        var updatedUser = _fakeUserDao.Users.Single(x => x.Id == testUser.Id);
        updatedUser.TimeZone.Value.Should().Be(newTimezone);
        updatedUser.Culture.Should().Be(currentCulture);
    }

    [Fact]
    public async Task UpdateUserProfile_OnlyCultureIsGiven_CultureUpdate()
    {
        // arrange
        var currentTimezone = Name.Parse("Europe/Berlin");
        var currentCulture = Name.Parse("de-AT");
        var testUser = new User(TestAccount.User.Id, TestAccount.User.Email, TestAccount.User.FirstName,
            TestAccount.User.LastName,
            currentTimezone, currentCulture, null, DateTime.Parse("2022-05-01"));

        const string newCulture = "en-US";

        _outputPortMock.Setup(m => m.Updated(It.IsAny<Guid>()));

        _fakeUserDao.Users.Add(testUser);
        var updateProfileRequestModel = new UpdateUserProfileRequestModel(testUser.Id, null, newCulture);
        var interactor = CreateInteractor();

        // act
        await interactor.ExecuteAsync(_outputPortMock.Object, updateProfileRequestModel);

        // assert
        _outputPortMock.Verify(m => m.Updated(It.Is<Guid>(x => x == testUser.Id)), Times.Once);
        var updatedUser = _fakeUserDao.Users.Single(x => x.Id == testUser.Id);
        updatedUser.TimeZone.Should().Be(currentTimezone);
        updatedUser.Culture.Value.Should().Be(newCulture);
    }
    
    [Fact]
    public async Task UpdateUserProfile_NotSupportedCulture_CultureNotFound()
    {
        // arrange
        var currentTimezone = Name.Parse("Europe/Berlin");
        var currentCulture = Name.Parse("de-AT");
        var testUser = new User(TestAccount.User.Id, TestAccount.User.Email, TestAccount.User.FirstName,
            TestAccount.User.LastName,
            currentTimezone, currentCulture, null, DateTime.Parse("2022-05-01"));

        const string newCulture = "de-VIENNA";

        _outputPortMock.Setup(m => m.CultureNotFound(It.IsAny<string>()));

        _fakeUserDao.Users.Add(testUser);
        var updateProfileRequestModel = new UpdateUserProfileRequestModel(testUser.Id, null, newCulture);
        var interactor = CreateInteractor();

        // act
        await interactor.ExecuteAsync(_outputPortMock.Object, updateProfileRequestModel);

        // assert
        _outputPortMock.Verify(m => m.CultureNotFound(It.Is<string>(x => x == newCulture)), Times.Once);
    }
        
    [Fact]
    public async Task UpdateUserProfile_NotSupportedTimezone_TimezoneNotFound()
    {
        // arrange
        var currentTimezone = Name.Parse("Europe/Berlin");
        var currentCulture = Name.Parse("de-AT");
        var testUser = new User(TestAccount.User.Id, TestAccount.User.Email, TestAccount.User.FirstName,
            TestAccount.User.LastName,
            currentTimezone, currentCulture, null, DateTime.Parse("2022-05-01"));

        const string newTimezone = "Europe/Stephansplatz";

        _outputPortMock.Setup(m => m.TimezoneNotFound(It.IsAny<string>()));

        _fakeUserDao.Users.Add(testUser);
        var updateProfileRequestModel = new UpdateUserProfileRequestModel(testUser.Id, newTimezone, null);
        var interactor = CreateInteractor();

        // act
        await interactor.ExecuteAsync(_outputPortMock.Object, updateProfileRequestModel);

        // assert
        _outputPortMock.Verify(m => m.TimezoneNotFound(It.Is<string>(x => x == newTimezone)), Times.Once);
    }
    
            
    [Fact]
    public async Task UpdateUserProfile_ErrorWhileUpdatingUser_ConflictOutput()
    {
        // arrange
        var currentTimezone = Name.Parse("Europe/Berlin");
        var currentCulture = Name.Parse("de-AT");
        var testUser = new User(TestAccount.User.Id, TestAccount.User.Email, TestAccount.User.FirstName,
            TestAccount.User.LastName,
            currentTimezone, currentCulture, null, DateTime.Parse("2022-05-01"));

        const string newTimezone = "Europe/Vienna";

        _outputPortMock.Setup(m => m.Conflict(It.IsAny<Conflict>()));

        _fakeUserDao.Users.Add(testUser); _fakeUserDao.ProduceFailure = true;
        var updateProfileRequestModel = new UpdateUserProfileRequestModel(testUser.Id, newTimezone, null);
        var interactor = CreateInteractor();

        // act
        await interactor.ExecuteAsync(_outputPortMock.Object, updateProfileRequestModel);

        // assert
        _outputPortMock.Verify(m => m.Conflict(It.IsAny<Conflict>()), Times.Once);
    }
}