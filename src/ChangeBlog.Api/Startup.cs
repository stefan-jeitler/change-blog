using System.Linq;
using ChangeBlog.Api.Authentication;
using ChangeBlog.Api.Authorization;
using ChangeBlog.Api.DTOs;
using ChangeBlog.Api.Extensions;
using ChangeBlog.Api.Shared.DTOs;
using ChangeBlog.Api.SwaggerUI;
using ChangeBlog.DataAccess.Postgres;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ChangeBlog.Api
{
    public class Startup
    {
        private readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        private static void ConfigureControllers(IServiceCollection services)
        {
            services
                .AddControllers(o => { o.Filters.Add(typeof(AuthorizationFilter)); })
                .ConfigureApiBehaviorOptions(o => o.InvalidModelStateResponseFactory = CustomErrorMessage);
        }

        public void ConfigureServices(IServiceCollection services)
        {
            ConfigureControllers(services);

            services.AddSwagger();
            services.AddApplicationInsightsTelemetry();
            services.AddApiKeyAuthentication();
            services.AddPermissionCheck();

            var connectionString = _configuration.GetConnectionString("ChangeBlogDb");
            services.AddPostgresDataAccess(connectionString);

            services.AddMemoryCache();
            services.AddUseCases();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
                app.UseDeveloperExceptionPage();

            app.AddSwagger();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
                endpoints
                    .MapControllers()
                    .RequireAuthorization());
        }

        private static ActionResult CustomErrorMessage(ActionContext context)
        {
            var firstError = context.ModelState
                .FirstOrDefault(modelError => modelError.Value.Errors.Count > 0)
                .Value.Errors.FirstOrDefault()?
                .ErrorMessage ?? "Unknown";

            return new BadRequestObjectResult(DefaultResponse.Create(firstError));
        }
    }
}
