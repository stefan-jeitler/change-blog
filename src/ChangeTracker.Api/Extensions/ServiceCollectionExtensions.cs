using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChangeTracker.Application.UseCases.Commands.AddProject;
using ChangeTracker.Application.UseCases.Commands.CloseProject;
using Microsoft.Extensions.DependencyInjection;

namespace ChangeTracker.Api.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddProjectUseCase(this IServiceCollection services) =>
            services
                .AddScoped<IAddProject, AddProjectInteractor>()
                .AddScoped<ICloseProject, CloseProjectInteractor>();
    }
}
