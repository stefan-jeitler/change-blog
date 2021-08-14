using ChangeBlog.Application.UseCases.Queries.GetVersions;
using Microsoft.Extensions.DependencyInjection;

namespace ChangeBlog.Api.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddUseCases(this IServiceCollection services)
        {
            return services.Scan(scan =>
                scan.FromAssemblyOf<IGetVersion>()
                    .AddClasses(f => f.Where(t => t.Name.EndsWith("Interactor")))
                    .AsImplementedInterfaces()
                    .WithScopedLifetime());
        }
    }
}
