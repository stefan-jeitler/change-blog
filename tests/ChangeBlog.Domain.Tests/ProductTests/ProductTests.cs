using System;
using ChangeBlog.Domain.Miscellaneous;
using ChangeBlog.Domain.Version;
using FluentAssertions;
using Xunit;

namespace ChangeBlog.Domain.Tests.ProductTests;

public class ProductTests
{
    private Guid _testAccountId;
    private DateTime _testCreationDate;
    private DateTime? _testFreezedDate;
    private Guid _testId;
    private Name _testLangCode;
    private Name _testName;
    private Guid _testUserId;
    private VersioningScheme _testVersioningScheme;

    public ProductTests()
    {
        _testId = Guid.Parse("992b6432-b792-4f0d-87c2-786f24a5a564");
        _testName = Name.Parse("ProductX");
        _testUserId = Guid.Parse("a1b89f2d-d13f-4572-8522-8a92fb4fdb6a");
        _testCreationDate = DateTime.Parse("2021-04-03");
        _testAccountId = TestAccount.Id;
        _testVersioningScheme = TestAccount.CustomVersioningScheme;
        _testLangCode = Name.Parse("en");
        _testFreezedDate = null;
    }

    private Product CreateProduct() =>
        new(_testId, _testAccountId, _testName, _testVersioningScheme, _testLangCode, _testUserId,
            _testCreationDate, _testFreezedDate);

    [Fact]
    public void Create_WithValidArguments_Successful()
    {
        var product = CreateProduct();

        product.Id.Should().Be(_testId);
        product.AccountId.Should().Be(_testAccountId);
        product.Name.Should().Be(_testName);
        product.VersioningScheme.Should().Be(_testVersioningScheme);
        product.CreatedByUser.Should().Be(_testUserId);
        product.CreatedAt.Should().Be(_testCreationDate);
        product.LanguageCode.Should().Be(_testLangCode);
        product.FreezedAt.HasValue.Should().BeFalse();
    }

    [Fact]
    public void Create_WithFreezedAtDate_DateProperlySet()
    {
        _testFreezedDate = DateTime.Parse("2021-04-16");

        var product = CreateProduct();

        product.FreezedAt.Should().HaveValue();
        product.FreezedAt!.Value.Should().Be(_testFreezedDate!.Value);
    }

    [Fact]
    public void FreezeProduct_HappyPath_DateProperlySet()
    {
        _testFreezedDate = null;
        var product = CreateProduct();

        var freezedProduct = product.Freeze();

        freezedProduct.FreezedAt.Should().HaveValue();
        freezedProduct.IsFreezed.Should().BeTrue();
    }

    [Fact]
    public void Create_WithEmptyId_ArgumentException()
    {
        _testId = Guid.Empty;

        var act = CreateProduct;

        act.Should().ThrowExactly<ArgumentException>();
    }

    [Fact]
    public void Create_WithEmptyAccountId_ArgumentException()
    {
        _testAccountId = Guid.Empty;

        var act = CreateProduct;

        act.Should().ThrowExactly<ArgumentException>();
    }

    [Fact]
    public void Create_WithNullName_ArgumentNullException()
    {
        _testName = null;

        var act = CreateProduct;

        act.Should().ThrowExactly<ArgumentNullException>();
    }

    [Fact]
    public void Create_WithNullVersioningScheme_ArgumentException()
    {
        _testVersioningScheme = null;

        var act = CreateProduct;

        act.Should().ThrowExactly<ArgumentNullException>();
    }

    [Fact]
    public void Create_WithNullLanguageCode_ArgumentException()
    {
        _testLangCode = null;

        var act = CreateProduct;

        act.Should().ThrowExactly<ArgumentNullException>();
    }

    [Fact]
    public void Create_WithEmptyUserId_ArgumentException()
    {
        _testUserId = Guid.Empty;

        var act = CreateProduct;

        act.Should().ThrowExactly<ArgumentException>();
    }

    [Theory]
    [InlineData("0001-01-01T00:00:00.0000000")]
    [InlineData("9999-12-31T23:59:59.9999999")]
    public void Create_WithInvalidCreatedAtDate_ArgumentException(string invalidDate)
    {
        _testCreationDate = DateTime.Parse(invalidDate);

        var act = CreateProduct;

        act.Should().ThrowExactly<ArgumentException>();
    }

    [Theory]
    [InlineData("0001-01-01T00:00:00.0000000")]
    [InlineData("9999-12-31T23:59:59.9999999")]
    public void Create_WithInvalidFreezedAtDate_ArgumentException(string invalidDate)
    {
        _testFreezedDate = DateTime.Parse(invalidDate);

        var act = CreateProduct;

        act.Should().ThrowExactly<ArgumentException>();
    }
}