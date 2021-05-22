using System;
using System.Configuration;
using System.Data;
using System.Net.Http.Json;
using System.Threading.Tasks;
using ChangeTracker.Api.DTOs.V1.Product;
using ChangeTracker.Api.Presenters.V1.Product;
using ChangeTracker.Application.UseCases.Commands.AddProduct;
using ChangeTracker.DataAccess.Postgres;
using Dapper;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

namespace ChangeTracker.Api.Tests
{
    public class ProductControllerTests : IClassFixture<WebApplicationFactory<Startup>>
    {
        private const string TestProductName = "AddProduct Controller Test";
        private static readonly Guid TestAccountId = Guid.Parse("ec3a44cc-0ba4-4c97-ad7f-911e9f6a73bc");
        
        private readonly WebApplicationFactory<Startup> _factory;

        public ProductControllerTests(WebApplicationFactory<Startup> factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task GetProducts_HappyPath_Successful()
        {
            // arrange
            var client = _factory.CreateClient();
            client.DefaultRequestHeaders.Add("X-API-KEY", new[] {"acc01usr02"});

            // act
            var response = await client.GetAsync("/api/v1/products/139a2e54-e9be-4168-98b4-2839d9b3db04");
            var content = await response.Content.ReadFromJsonAsync<ProductDto>();

            // assert
            response.StatusCode.Should().Be(StatusCodes.Status200OK);
            content.Id.Should().Be(Guid.Parse("139a2e54-e9be-4168-98b4-2839d9b3db04"));
        }

        [Fact]
        public async Task AddProduct_HappyPath_ProductAdded()
        {
            // arrange
            var acquireDbConnection = _factory.Services.GetRequiredService<Func<IDbConnection>>();
            await DeleteTestProductAsync(acquireDbConnection);
            var client = _factory.CreateClient();
            client.DefaultRequestHeaders.Add("X-API-KEY", new[] {"acc01usr02"});

            var product = new AddProductDto
            {
                Name = TestProductName,
                AccountId = TestAccountId
            };

            // act
            var response = await client.PostAsJsonAsync("api/v1/products", product);
            
            // arrange
            response.StatusCode.Should().Be(StatusCodes.Status201Created);
        }
        
        [Fact]
        public async Task AddProduct_EmptyVersioningSchemeId_BadRequest()
        {
            // arrange
            var acquireDbConnection = _factory.Services.GetRequiredService<Func<IDbConnection>>();
            await DeleteTestProductAsync(acquireDbConnection);
            var client = _factory.CreateClient();
            client.DefaultRequestHeaders.Add("X-API-KEY", new[] {"acc01usr02"});

            var product = new AddProductDto
            {
                Name = TestProductName,
                AccountId = TestAccountId,
                VersioningSchemeId = Guid.Empty
            };

            // act
            var response = await client.PostAsJsonAsync("api/v1/products", product);
            
            // arrange
            response.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
        }
        
        [Fact]
        public async Task CloseProduct_HappyPath_NoContent()
        {
            // arrange
            var client = _factory.CreateClient();
            client.DefaultRequestHeaders.Add("X-API-KEY", new[] {"acc01usr02"});

            // act
            var response = await client.PostAsync("api/v1/products/139a2e54-e9be-4168-98b4-2839d9b3db04/close", null!);
            
            // arrange
            response.StatusCode.Should().Be(StatusCodes.Status204NoContent);
        }
        
        private static async Task DeleteTestProductAsync(Func<IDbConnection> acquireDbConnection)
        {
            using var dbConnection = acquireDbConnection();
            await dbConnection.ExecuteAsync("delete from product where account_id = @accountId and name = @name",
                new
                {
                    accountId = TestAccountId,
                    name = TestProductName
                });
        }
    }
}