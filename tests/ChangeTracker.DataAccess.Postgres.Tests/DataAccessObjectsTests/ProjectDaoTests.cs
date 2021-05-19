using System;
using System.Threading.Tasks;
using ChangeTracker.Application.DataAccess.Products;
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
    public class ProductDaoTests : IDisposable
    {
        private readonly LazyDbConnection _lazyDbConnection;

        public ProductDaoTests()
        {
            _lazyDbConnection = new LazyDbConnection(() => new NpgsqlConnection(Configuration.ConnectionString));
        }

        public void Dispose()
        {
            _lazyDbConnection?.Dispose();
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
            product.Value.Id.Should().Be(t_ua_account_01_proj_02);
        }

        [Fact]
        public async Task FindProduct_ByProductId_ReturnsProduct()
        {
            var productDao = CreateDao();
            var t_ua_account_01_proj_02 = Guid.Parse("0614f8d6-8895-4c74-bcbe-8a3c26076e1b");

            var product =
                await productDao.FindProductAsync(t_ua_account_01_proj_02);

            product.HasValue.Should().BeTrue();
            product.Value.Id.Should().Be(t_ua_account_01_proj_02);
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
        public void GetProduct_NotExistingProduct_Exception()
        {
            var productDao = CreateDao();
            var notExistingProductId = Guid.Parse("21f05095-c016-4f60-b98a-03c037b6cc8c");

            Func<Task<Product>> act = () => productDao.GetProductAsync(notExistingProductId);

            act.Should().ThrowExactly<Exception>();
        }

        [Fact]
        public async Task GetProducts_HappyPath_ReturnsProducts()
        {
            var productDao = CreateDao();
            var t_ua_account_01 = Guid.Parse("ec3a44cc-0ba4-4c97-ad7f-911e9f6a73bc");
            var t_ua_account_01_user_02 = Guid.Parse("7aa9004b-ed6f-4862-8307-579030c860be");
            var querySettings = new ProductQuerySettings(t_ua_account_01, t_ua_account_01_user_02);

            var products = await productDao.GetProductsAsync(querySettings);

            products.Should().HaveCount(2);
        }

        [Fact]
        public async Task GetProducts_LimitResultBy1_ReturnsOnlyOneProduct()
        {
            var productDao = CreateDao();
            var t_ua_account_01 = Guid.Parse("ec3a44cc-0ba4-4c97-ad7f-911e9f6a73bc");
            var t_ua_account_01_user_02 = Guid.Parse("7aa9004b-ed6f-4862-8307-579030c860be");
            var querySettings = new ProductQuerySettings(t_ua_account_01, t_ua_account_01_user_02, null, 1);

            var products = await productDao.GetProductsAsync(querySettings);

            products.Should().HaveCount(1);
        }

        [Fact]
        public async Task GetProducts_SkipFirstProduct_ReturnsSecond()
        {
            var productDao = CreateDao();
            var t_ua_account_01 = Guid.Parse("ec3a44cc-0ba4-4c97-ad7f-911e9f6a73bc");
            var t_ua_account_01_user_02 = Guid.Parse("7aa9004b-ed6f-4862-8307-579030c860be");
            var lastProductId = Guid.Parse("139a2e54-e9be-4168-98b4-2839d9b3db04");
            var querySettings = new ProductQuerySettings(t_ua_account_01, t_ua_account_01_user_02, lastProductId);

            var products = await productDao.GetProductsAsync(querySettings);

            products.Should().HaveCount(1);
            products.Should().Contain(x => x.Id == Guid.Parse("0614f8d6-8895-4c74-bcbe-8a3c26076e1b"));
        }
    }
}