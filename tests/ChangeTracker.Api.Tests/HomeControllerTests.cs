using System;
using System.Net;
using System.Threading.Tasks;
using ChangeTracker.Api.Authentication;
using ChangeTracker.Api.Authentication.AccountDaos;
using ChangeTracker.Api.DTOs;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Xunit;

namespace ChangeTracker.Api.Tests
{
    public class HomeControllerTests : IClassFixture<WebApplicationFactory<Startup>>
    {
        private readonly WebApplicationFactory<Startup> _factory;

        public HomeControllerTests(WebApplicationFactory<Startup> factory)
        {
            _factory = factory;
        }

        [Theory]
        [InlineData("swagger")]
        [InlineData("changeLog")]
        public async Task IndexEndpoint_ContainsLinkTo(string relativePath)
        {
            // arrange
            var client = _factory.CreateClient();
            Func<string, bool> endsWithSwagger =
                c => c.TrimEnd('/').EndsWith(relativePath, StringComparison.OrdinalIgnoreCase);

            // act
            var response = await client.GetAsync("/");
            var apiInfo = JsonConvert.DeserializeObject<ApiInfo>(await response.Content.ReadAsStringAsync());

            // assert
            apiInfo.ImportantLinks.Should().Contain(x => endsWithSwagger(x));
        }

        [Fact]
        public async Task IndexEndpoint_DeserializeApiInfo_SuccessfullyDeserialized()
        {
            var client = _factory.CreateClient();

            var response = await client.GetAsync("/");

            var apiInfo = JsonConvert.DeserializeObject<ApiInfo>(await response.Content.ReadAsStringAsync());
            apiInfo.Should().NotBeNull();
        }

        [Fact]
        public async Task IndexEndpoint_Exists()
        {
            var client = _factory.CreateClient();

            var response = await client.GetAsync("/");

            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task ChangeLogEndpoint_NoApiKeyProvided_Unauthorized()
        {
            var client = _factory.CreateClient();

            var response = await client.GetAsync("/changeLog");

            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task ChangeLogEndpoint_CorrectApiKeyIsPresent_Authorized()
        {
            // arrange
            const string testApiKey = "test-api-key";
            var client = _factory
                .WithWebHostBuilder(whb => whb.ConfigureServices(s =>
                    s.AddSingleton<IAccountDao>(_ => new InMemoryAccountDao(testApiKey))))
                .CreateClient();
            client.DefaultRequestHeaders.Add("X-API-KEY", new[] {testApiKey});

            // act
            var response = await client.GetAsync("/changeLog");

            // assert
            response.StatusCode.Should().NotBe(HttpStatusCode.Unauthorized);
        }
    }
}