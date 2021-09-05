using System.Net.Http;
using ChangeBlog.Application.DataAccess.ExternalIdentity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Identity.Web;

namespace ChangeBlog.DataAccess.MicrosoftIdentity
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddMicrosoftIdentityDataAccess(this IServiceCollection services,
            string userInfoEndpointBaseUrl)
        {
            var httpClient = new HttpClient();
            services.AddScoped<IExternalUserInfoDao>(sp => new ExternalUserInfoDao(
                sp.GetRequiredService<ITokenAcquisition>(),
                httpClient,
                userInfoEndpointBaseUrl));

            return services;
        }
    }
}