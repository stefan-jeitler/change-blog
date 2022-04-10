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
        var httpClient = new HttpClient { BaseAddress = new Uri(userInfoEndpointBaseUrl) };

        services.AddScoped<IExternalUserInfoDao>(sp => new ExternalUserInfoDao(
            sp.GetRequiredService<ITokenAcquisition>(),
            httpClient));

        return services;
    }
}