using System;
using FluentAssertions;
using Xunit;

namespace ChangeBlog.Domain.Tests;

public class VersioningSchemeIdFinderTests
{
    [Fact]
    public void FindSchemeIdForProduct_NoCustomSchemeIdPresentAndNoAccountDefaultScheme_ReturnsDefaultSchemeId()
    {
        // arrange
        var testAccount = new Account(TestAccount.Id, TestAccount.Name, null, TestAccount.CreationDate, null);
        var service = new VersioningSchemeIdFinder(testAccount);

        // act
        var schemeId = service.FindSchemeIdForProduct(null);

        // assert
        schemeId.Should().Be(Default.VersioningSchemeId);
    }

    [Fact]
    public void
        FindSchemeIdForProduct_NoCustomSchemeIdPresentAndAccountHasDefaultScheme_ReturnsAccountDefaultSchemeId()
    {
        // arrange
        var testAccount = new Account(TestAccount.Id, TestAccount.Name, TestAccount.CustomVersioningScheme.Id,
            TestAccount.CreationDate, null);
        var customSchemeId = (Guid?)null;

        var service = new VersioningSchemeIdFinder(testAccount);

        // act
        var schemeId = service.FindSchemeIdForProduct(customSchemeId);

        // assert
        schemeId.Should().Be(TestAccount.CustomVersioningScheme.Id);
    }

    [Fact]
    public void
        FindSchemeIdForProduct_CustomSchemeIdPresentAndAccountHasDefaultSchemeId_ReturnsCustomDefaultSchemeId()
    {
        // arrange
        var testAccount = new Account(TestAccount.Id, TestAccount.Name, TestAccount.CustomVersioningScheme.Id,
            TestAccount.CreationDate, null);
        var customSchemeId = Guid.Parse("aaf9047d-1086-4d57-82cd-3325592a0d27");

        var service = new VersioningSchemeIdFinder(testAccount);

        // act
        var schemeId = service.FindSchemeIdForProduct(customSchemeId);

        // assert
        schemeId.Should().Be(customSchemeId);
    }
}