using System.Linq;
using ChangeBlog.Api.Shared;
using ChangeBlog.Api.Shared.DTOs;
using ChangeBlog.DataAccess.MicrosoftIdentity;
using ChangeBlog.DataAccess.Postgres;
using ChangeBlog.Management.Api.Authentication;
using ChangeBlog.Management.Api.Extensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SpaServices.AngularCli;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ChangeBlog.Management.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            var settings = Configuration
                .GetSection("Authentication:MicrosoftIdentity")
                .Get<MicrosoftIdentityAuthenticationSettings>();

            services
                .AddAuthenticationServices()
                .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddAppAuthentication(settings);

            services
                .AddControllers()
                .ConfigureApiBehaviorOptions(o => o.InvalidModelStateResponseFactory = CustomErrorMessage);

            services.AddSpaStaticFiles(configuration => { configuration.RootPath = "ClientApp/dist"; });

            services
                .AddSwagger()
                .AddApplicationUseCases();

            var connectionString = Configuration.GetConnectionString("ChangeBlogDb");
            services.AddPostgresDataAccess(connectionString);

            var userInfoEndpointBaseUrl = Configuration.GetValue<string>("UserInfoEndpointBaseUrl");
            services.AddMicrosoftIdentityDataAccess(userInfoEndpointBaseUrl);
        }


        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.AddSwagger();

            app.UseStaticFiles();
            if (!env.IsDevelopment())
            {
                app.UseSpaStaticFiles();
            }

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints
                    .MapControllers()
                    .RequireAuthorization();
            });

            app.UseSpa(spa =>
            {
                spa.Options.SourcePath = "ClientApp";

                if (env.IsDevelopment())
                {
                    spa.UseAngularCliServer("start");
                }
            });
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