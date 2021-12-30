using System;
using System.Threading.Tasks;
using ChangeBlog.Application.Boundaries.DataAccess.Products;
using ChangeBlog.DataAccess.Postgres.DataAccessObjects.Products;
using ChangeBlog.Domain.Miscellaneous;
using Dapper;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Npgsql;
using Xunit;

// ReSharper disable InconsistentNaming

namespace ChangeBlog.DataAccess.Postgres.Tests.DataAccessObjectsTests;

public class ProductDaoTests : IAsyncLifetime
{
    private readonly LazyDbConnection _lazyDbConnection;

    public ProductDaoTests()
    {
        _lazyDbConnection = new LazyDbConnection(() => new NpgsqlConnection(Configuration.ConnectionString));
    }

    public async Task InitializeAsync()
    {
        await InsertTestAccountAsync();
        await InsertTestUserAsync();
    }

    public Task DisposeAsync()
    {
        _lazyDbConnection?.Dispose();
        return Task.CompletedTask;
    }

    private ProductDao CreateDao()
    {
        return new(new DbSession(_lazyDbConnection), NullLogger<ProductDao>.Instance);
    }

    [Fact]
    public async Task FindProduct_ByAccountIdAndName_ReturnsProduct()
    {
        var productDao = CreateDao();
        var t_ua_account_01_proj_02 = Guid.Parse("0614f8d6-8895-4c74-bcbe-8a3c26076e1b");
        var t_ua_account_01 = Guid.Parse("ec3a44cc-0ba4-4c97-ad7f-911e9f6a73bc");

        var product =
            await productDao.FindProductAsync(t_ua_account_01, Name.Parse(nameof(t_ua_account_01_proj_02)));

        product.HasValue.Should().BeTrue();
        product.GetValueOrThrow().Id.Should().Be(t_ua_account_01_proj_02);
    }

    [Fact]
    public async Task FindProduct_ByProductId_ReturnsProduct()
    {
        var productDao = CreateDao();
        var t_ua_account_01_proj_02 = Guid.Parse("0614f8d6-8895-4c74-bcbe-8a3c26076e1b");

        var product =
            await productDao.FindProductAsync(t_ua_account_01_proj_02);

        product.HasValue.Should().BeTrue();
        product.GetValueOrThrow().Id.Should().Be(t_ua_account_01_proj_02);
    }

    [Fact]
    public async Task GetProduct_ExistingProduct_ReturnsProduct()
    {
        var productDao = CreateDao();
        var t_ua_account_01_proj_02 = Guid.Parse("0614f8d6-8895-4c74-bcbe-8a3c26076e1b");

        var product = await productDao.GetProductAsync(t_ua_account_01_proj_02);

        product.Id.Should().Be(t_ua_account_01_proj_02);
    }

    [Fact]
    public async Task GetProduct_NotExistingProduct_Exception()
    {
        var productDao = CreateDao();
        var notExistingProductId = Guid.Parse("21f05095-c016-4f60-b98a-03c037b6cc8c");

        var act = () => productDao.GetProductAsync(notExistingProductId);

        await act.Should().ThrowExactlyAsync<Exception>();
    }

    [Fact]
    public async Task GetProducts_HappyPath_ReturnsProducts()
    {
        var productDao = CreateDao();
        var t_ua_account_01 = Guid.Parse("ec3a44cc-0ba4-4c97-ad7f-911e9f6a73bc");
        var t_ua_account_01_user_02 = Guid.Parse("7aa9004b-ed6f-4862-8307-579030c860be");
        var querySettings =
            new AccountProductsQuerySettings(t_ua_account_01, t_ua_account_01_user_02, null, 100, true);

        var products = await productDao.GetAccountProductsAsync(querySettings);

        products.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetProducts_LimitResultBy1_ReturnsOnlyOneProduct()
    {
        var productDao = CreateDao();
        var t_ua_account_01 = Guid.Parse("ec3a44cc-0ba4-4c97-ad7f-911e9f6a73bc");
        var t_ua_account_01_user_02 = Guid.Parse("7aa9004b-ed6f-4862-8307-579030c860be");
        var querySettings = new AccountProductsQuerySettings(t_ua_account_01, t_ua_account_01_user_02, null, 1);

        var products = await productDao.GetAccountProductsAsync(querySettings);

        products.Should().HaveCount(1);
    }

    [Fact]
    public async Task GetProducts_SkipFirstProduct_ReturnsSecond()
    {
        var productDao = CreateDao();
        var t_ua_account_01 = Guid.Parse("ec3a44cc-0ba4-4c97-ad7f-911e9f6a73bc");
        var t_ua_account_01_user_02 = Guid.Parse("7aa9004b-ed6f-4862-8307-579030c860be");
        var lastProductId = Guid.Parse("139a2e54-e9be-4168-98b4-2839d9b3db04");
        var querySettings =
            new AccountProductsQuerySettings(t_ua_account_01, t_ua_account_01_user_02, lastProductId);

        var products = await productDao.GetAccountProductsAsync(querySettings);

        products.Should().HaveCount(1);
        products.Should().Contain(x => x.Id == Guid.Parse("0614f8d6-8895-4c74-bcbe-8a3c26076e1b"));
    }

    [Fact]
    public async Task AddProduct_HappyPath_SuccessfullyAdded()
    {
        // arrange
        var productDao = CreateDao();
        await _lazyDbConnection.Value.ExecuteAsync("delete from product where id = @productId", new
        {
            productId = TestData.Product.Id
        });

        // act
        var result = await productDao.AddProductAsync(TestData.Product);

        // assert
        result.IsSuccess.Should().BeTrue();
        var exists = await _lazyDbConnection.Value.ExecuteScalarAsync<bool>(
            "select exists(select null from product where id = @productId)", new { productId = TestData.Product.Id });
        exists.Should().BeTrue();
    }

    [Fact]
    public async Task CloseProduct_HappyPath_SuccessfullyClosed()
    {
        // arrange
        var productDao = CreateDao();
        await _lazyDbConnection.Value.ExecuteAsync("update product set closed_at = null where id = @productId", new
        {
            productId = TestData.Product.Id
        });

        // act
        var closedProduct = TestData.Product.Close();
        await productDao.CloseProductAsync(closedProduct);

        // assert
        var exists = await _lazyDbConnection.Value.ExecuteScalarAsync<bool>(
            "select exists(select null from product where id = @productId and closed_at is not null)",
            new { productId = TestData.Product.Id });
        exists.Should().BeTrue();
    }

    private async Task InsertTestUserAsync()
    {
        const string insertTestUserSql =
            @"insert into ""user"" values (@id, @email, @firstName, @lastName, @timeZone, @deletedAt, @createdAt) on conflict (id) do nothing";
        await _lazyDbConnection.Value.ExecuteAsync(insertTestUserSql, new
        {
            id = TestData.User.Id,
            email = TestData.User.Email,
            firstName = TestData.User.FirstName,
            lastName = TestData.User.LastName,
            timeZone = TestData.User.TimeZone,
            deletedAt = TestData.User.DeletedAt,
            createdAt = TestData.User.CreatedAt
        });
    }

    private async Task InsertTestAccountAsync()
    {
        const string insertTestAccountSql =
            "insert into account values (@id, @name, @versioningSchemeId, @deletedAt, @createdAt) on conflict (id) do nothing";
        await _lazyDbConnection.Value.ExecuteAsync(insertTestAccountSql,
            new
            {
                id = TestData.Account.Id,
                name = TestData.Account.Name,
                versioningSchemeId = TestData.Account.DefaultVersioningSchemeId,
                deletedAt = TestData.Account.DeletedAt,
                createdAt = TestData.Account.CreatedAt
            });
    }
}