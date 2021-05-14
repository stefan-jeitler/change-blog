using ChangeTracker.Application.UseCases.Commands.AddProject;
using ChangeTracker.Application.UseCases.Commands.CloseProject;
using ChangeTracker.Application.UseCases.Queries.GetProjects;
using ChangeTracker.Application.UseCases.Queries.GetRoles;
using Microsoft.Extensions.DependencyInjection;

namespace ChangeTracker.Api.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddProjectUseCase(this IServiceCollection services) =>
            services
                .AddScoped<IAddProject, AddProjectInteractor>()
                .AddScoped<ICloseProject, CloseProjectInteractor>()
                .AddScoped<IGetProjects, GetProjectsInteractor>()
                .AddScoped<IGetRoles, GetRolesInteractor>();
    }
}