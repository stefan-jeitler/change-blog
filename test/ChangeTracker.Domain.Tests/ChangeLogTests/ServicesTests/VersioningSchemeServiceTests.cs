using System;
using ChangeTracker.Domain.ChangeLog.Services;
using ChangeTracker.Domain.Tests.TestDoubles;
using ChangeTracker.Domain.Version;
using FluentAssertions;
using Xunit;

namespace ChangeTracker.Domain.Tests.ChangeLogTests.ServicesTests
{
    public class VersioningSchemeServiceTests
    {
        [Fact]
        public void FindSchemeIdForProject_NoCustomSchemeIdPresentAndNotAccountDefaultScheme_ReturnsDefaultSchemeId()
        {
            // arrange
            var testAccount = new Account(TestAccount.Id, TestAccount.Name, null, TestAccount.CreationDate, null);
            var customSchemeId = (Guid?) null;

            var service = new VersioningSchemeService(testAccount);

            // act
            var schemeId = service.FindSchemeIdForProject(customSchemeId);

            // assert
            schemeId.Should().Be(Defaults.VersioningScheme.Id);
        }

        [Fact]
        public void
            FindSchemeIdForProject_NoCustomSchemeIdPresentAndAccountHasDefaultScheme_ReturnsAccountDefaultSchemeId()
        {
            // arrange
            var testAccount = new Account(TestAccount.Id, TestAccount.Name, TestAccount.CustomVersioningScheme.Id,
                TestAccount.CreationDate, null);
            var customSchemeId = (Guid?) null;

            var service = new VersioningSchemeService(testAccount);

            // act
            var schemeId = service.FindSchemeIdForProject(customSchemeId);

            // assert
            schemeId.Should().Be(TestAccount.CustomVersioningScheme.Id);
        }

        [Fact]
        public void
            FindSchemeIdForProject_CustomSchemeIdPresentAndAccountHasDefaultSchemeId_ReturnsCustomDefaultSchemeId()
        {
            // arrange
            var testAccount = new Account(TestAccount.Id, TestAccount.Name, TestAccount.CustomVersioningScheme.Id,
                TestAccount.CreationDate, null);
            var customSchemeId = Guid.Parse("aaf9047d-1086-4d57-82cd-3325592a0d27");

            var service = new VersioningSchemeService(testAccount);

            // act
            var schemeId = service.FindSchemeIdForProject(customSchemeId);

            // assert
            schemeId.Should().Be(customSchemeId);
        }
    }
}