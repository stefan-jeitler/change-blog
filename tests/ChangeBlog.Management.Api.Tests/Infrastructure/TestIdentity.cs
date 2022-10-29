using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace ChangeBlog.Management.Api.Tests.Infrastructure;

public static class TestIdentity
{
    private static readonly Lazy<Task<string>> AccessTokenValue = new(LoadAccessToken);
    public static Task<string> AccessToken => AccessTokenValue.Value;

    private static async Task<string> LoadAccessToken()
    {
        var config = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json")
            .AddUserSecrets<HomeControllerTests>()
            .AddEnvironmentVariables()
            .Build();

        var testIdentity = config
            .GetSection("TestIdentity")
            .Get<RopcFlowConfiguration>();

        var tokenClient = new RopcFlowClient();

        var tokenResponse = await tokenClient.AcquireToken(testIdentity);
        return tokenResponse.AccessToken;
    }
}