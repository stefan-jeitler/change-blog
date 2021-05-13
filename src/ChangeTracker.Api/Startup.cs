using ChangeTracker.Api.Authentication;
using ChangeTracker.Api.Authorization;
using ChangeTracker.Api.Extensions;
using ChangeTracker.Api.SwaggerUI;
using ChangeTracker.DataAccess.Postgres;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ChangeTracker.Api
{
    public class Startup
    {
        private readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        private static void AddUseCases(IServiceCollection services)
        {
            services.AddProjectUseCase();
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers(o => o.Filters.Add(typeof(PermissionAuthorizationFilter)));
            services.AddSwagger();

            services.AddApplicationInsightsTelemetry();

            services.AddApiKeyAuthentication();
            services.AddPermissionCheck();

            var connectionString = _configuration.GetConnectionString("ChangeTrackerDb");
            services.AddPostgresDataAccess(connectionString);

            AddUseCases(services);
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
                app.UseDeveloperExceptionPage();

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "ChangeTracker.Api v1");
                c.RoutePrefix = string.Empty;
            });

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
                endpoints
                    .MapControllers()
                    .RequireAuthorization());
        }
    }
}