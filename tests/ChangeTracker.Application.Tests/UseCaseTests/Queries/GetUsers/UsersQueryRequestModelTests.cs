﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChangeTracker.Application.Tests.TestDoubles;
using ChangeTracker.Application.UseCases.Queries.GetProjects;
using ChangeTracker.Application.UseCases.Queries.GetUsers;
using FluentAssertions;
using Xunit;

namespace ChangeTracker.Application.Tests.UseCaseTests.Queries.GetUsers
{
    public class UsersQueryRequestModelTests
    {

        private Guid _testUserId;
        private Guid _testAccountId;
        private readonly Guid? _testLastUserId;
        private ushort _testCount;


        public UsersQueryRequestModelTests()
        {
            _testUserId = TestAccount.UserId;
            _testAccountId = TestAccount.Id;
            _testLastUserId = Guid.Parse("33f9a7a4-5a0b-4ac8-b074-d97e13e8596c");
            _testCount = 100;
        }

        private UsersQueryRequestModel CreateRequestModel() =>
            new(_testUserId, _testAccountId, _testLastUserId, _testCount);

        [Fact]
        public void Create_HappyPath_Successful()
        {
            var requestModel = CreateRequestModel();

            requestModel.UserId.Should().Be(_testUserId);
            requestModel.AccountId.Should().Be(_testAccountId);
            requestModel.LastUserId.Should().Be(_testLastUserId);
            requestModel.Count.Should().Be(_testCount);
        }

        [Fact]
        public void Create_WithEmptyUserId_ArgumentException()
        {
            // arrange
            _testUserId = Guid.Empty;

            // act
            Func<UsersQueryRequestModel> act = CreateRequestModel;

            // assert
            act.Should().ThrowExactly<ArgumentException>();
        }

        [Fact]
        public void Create_WithEmptyAccountId_ArgumentException()
        {
            // arrange
            _testAccountId = Guid.Empty;

            // act
            Func<UsersQueryRequestModel> act = CreateRequestModel;

            // assert
            act.Should().ThrowExactly<ArgumentException>();
        }

        [Fact]
        public void Create_WithTooHighCountValue_ValueCapped()
        {
            // arrange
            _testCount = 500;

            // act
            var requestModel = CreateRequestModel();

            // assert
            requestModel.Count.Should().Be(UsersQueryRequestModel.MaxChunkCount);
        }
    }
}
