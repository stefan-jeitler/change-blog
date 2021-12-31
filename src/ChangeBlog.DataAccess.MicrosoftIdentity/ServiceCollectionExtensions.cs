using System;
using System.Net.Http;
using ChangeBlog.Application.Boundaries.DataAccess.ExternalIdentity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Identity.Web;

namespace ChangeBlog.DataAccess.MicrosoftIdentity;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddMicrosoftIdentityDataAccess(this IServiceCollection services,
        string userInfoEndpointBaseUrl)
    {
        var userInfoEndpoint = new Uri($"{userInfoEndpointBaseUrl.TrimEnd('/')}/userinfo");
        var httpClient = new HttpClient { BaseAddress = userInfoEndpoint };

        services.AddScoped<IExternalUserInfoDao>(sp => new ExternalUserInfoDao(
            sp.GetRequiredService<ITokenAcquisition>(),
            httpClient));

        return services;
    }
}