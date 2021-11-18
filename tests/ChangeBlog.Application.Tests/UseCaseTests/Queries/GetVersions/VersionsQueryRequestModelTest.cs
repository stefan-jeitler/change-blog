using System;
using ChangeBlog.Application.UseCases.Queries.GetVersions;
using FluentAssertions;
using Xunit;

namespace ChangeBlog.Application.Tests.UseCaseTests.Queries.GetVersions
{
    public class VersionsQueryRequestModelTest
    {
        private bool _testIncludeDeleted;
        private Guid? _testLastVersionId;
        private ushort _testLimit;
        private Guid _testProductId;
        private string _testSearchTerm;
        private Guid _testUserId;

        public VersionsQueryRequestModelTest()
        {
            _testProductId = TestAccount.Product.Id;
            _testLastVersionId = Guid.Parse("5c6af1ab-2b93-4e42-80c2-026a611076f1");
            _testUserId = TestAccount.UserId;
            _testSearchTerm = string.Empty;
            _testLimit = 1;
            _testIncludeDeleted = false;
        }

        private VersionsQueryRequestModel CreateRequestModel() => new(_testProductId, _testLastVersionId, _testUserId,
            _testSearchTerm, _testLimit, _testIncludeDeleted);

        [Fact]
        public void Create_ValidModel_ArgumentsProperlyAssigned()
        {
            var requestModel = CreateRequestModel();

            requestModel.ProductId.Should().Be(_testProductId);
            requestModel.LastVersionId.Should().Be(_testLastVersionId);
            requestModel.UserId.Should().Be(_testUserId);
            requestModel.SearchTerm.Should().Be(_testSearchTerm);
            requestModel.Limit.Should().Be(_testLimit);
            requestModel.IncludeDeleted.Should().Be(_testIncludeDeleted);
        }

        [Fact]
        public void Create_WithNullLastVersionId_NullIsAllowed()
        {
            _testLastVersionId = null;

            var requestModel = CreateRequestModel();

            requestModel.LastVersionId.Should().BeNull();
        }

        [Fact]
        public void Create_WithEmptyProductId_ArgumentException()
        {
            _testProductId = Guid.Empty;

            Func<VersionsQueryRequestModel> act = CreateRequestModel;

            act.Should().ThrowExactly<ArgumentException>();
        }

        [Fact]
        public void Create_WithEmptyUserId_ArgumentException()
        {
            _testUserId = Guid.Empty;

            Func<VersionsQueryRequestModel> act = CreateRequestModel;

            act.Should().ThrowExactly<ArgumentException>();
        }

        [Fact]
        public void Create_WithNullSearchTerm_NullIsAllowed()
        {
            _testSearchTerm = null;

            var requestModel = CreateRequestModel();

            requestModel.SearchTerm.Should().BeNull();
        }
        
        [Fact]
        public void Create_WithZeroLimit_ArgumentException()
        {
            _testLimit = 0;

            Func<VersionsQueryRequestModel> act = CreateRequestModel;

            act.Should().ThrowExactly<ArgumentException>();
        }
        
        [Fact]
        public void Create_IncludeDeleted_IncludeDeletedIsTrue()
        {
            _testIncludeDeleted = true;

            var requestModel = CreateRequestModel();

            requestModel.IncludeDeleted.Should().BeTrue();
        }
    }
}