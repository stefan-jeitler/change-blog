﻿using System;
using System.Threading.Tasks;
using ChangeTracker.Application.DataAccess;
using ChangeTracker.Application.Tests.TestDoubles;
using ChangeTracker.Application.UseCases.Commands.AddOrUpdateVersion;
using ChangeTracker.Application.UseCases.Commands.AddVersion;
using ChangeTracker.Application.UseCases.Commands.SharedModels;
using ChangeTracker.Domain.Common;
using ChangeTracker.Domain.Version;
using FluentAssertions;
using Moq;
using Xunit;

namespace ChangeTracker.Application.Tests.UseCaseTests.Commands.AddOrUpdateVersion
{
    public class AddOrUpdateVersionInteractorTests
    {
        private readonly Mock<IAddVersion> _addVersionMock;
        private readonly ProductDaoStub _productDaoStub;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly VersionDaoStub _versionDaoStub;
        private readonly Mock<IAddOrUpdateVersionOutputPort> _outputPortMock;

        public AddOrUpdateVersionInteractorTests()
        {
            _productDaoStub = new ProductDaoStub();
            _versionDaoStub = new VersionDaoStub();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _addVersionMock = new Mock<IAddVersion>();
            _outputPortMock = new Mock<IAddOrUpdateVersionOutputPort>(MockBehavior.Strict);
        }

        private AddOrUpdateVersionInteractor CreateInteractor() => new(_versionDaoStub,
            _productDaoStub, _unitOfWorkMock.Object, _addVersionMock.Object);

        [Fact]
        public async Task UpdateVersion_HappyPath_SuccessfullyUpdated()
        {
            // arrange
            var requestModel =
                new VersionRequestModel(TestAccount.UserId, TestAccount.Product.Id, "1.2.3", "catchy name");

            var existingVersion = new ClVersion(TestAccount.Product.Id, ClVersionValue.Parse("1.2.3"),
                OptionalName.Empty, TestAccount.UserId);

            _versionDaoStub.Versions.Add(existingVersion);
            _productDaoStub.Products.Add(TestAccount.Product);
            var interactor = CreateInteractor();

            _outputPortMock.Setup(m => m.VersionUpdated(It.IsAny<Guid>()));

            // act
            await interactor.ExecuteAsync(_outputPortMock.Object, requestModel);

            // assert
            _outputPortMock.Verify(m => m.VersionUpdated(It.Is<Guid>(x => x == existingVersion.Id)), Times.Once);
            _versionDaoStub.Versions.Should().HaveCount(1);
            _versionDaoStub.Versions.Should().Contain(x => x.Id == existingVersion.Id && x.Name == "catchy name");
        }
    }
}