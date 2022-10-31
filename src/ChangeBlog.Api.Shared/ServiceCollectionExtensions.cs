using ChangeBlog.Application.UseCases.Versions.GetVersions;
using Microsoft.Extensions.DependencyInjection;

namespace ChangeBlog.Api.Shared;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationUseCases(this IServiceCollection services)
    {
        return services.Scan(scan =>
            scan.FromAssemblyOf<IGetVersion>()
                .AddClasses(f => f.Where(t => t.Name.EndsWith("Interactor")))
                .AsImplementedInterfaces()
                .WithScopedLifetime());
    }
}