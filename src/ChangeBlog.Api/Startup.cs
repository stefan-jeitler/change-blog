using System.Linq;
using ChangeBlog.Api.Authentication;
using ChangeBlog.Api.Shared;
using ChangeBlog.Api.Shared.Authentication;
using ChangeBlog.Api.Shared.Authorization;
using ChangeBlog.Api.Shared.DTOs;
using ChangeBlog.Api.Shared.UserInfo;
using ChangeBlog.Api.Swagger;
using ChangeBlog.Application.Boundaries.DataAccess.ExternalIdentity;
using ChangeBlog.DataAccess.Postgres;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ChangeBlog.Api;

public class Startup
{
    private const string AllowManagementAppOrigin = nameof(AllowManagementAppOrigin);
    private readonly IConfiguration _configuration;

    public Startup(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public void ConfigureServices(IServiceCollection services)
    {
        ConfigureControllers(services);

        services.AddSwagger();
        services.AddApplicationInsightsTelemetry();

        var settings = _configuration
            .GetSection("Authentication:AzureAdB2C")
            .Get<MicrosoftIdentityAuthenticationSettings>();

        services.AddAppAuthentication(settings);
        services.AddPermissionHandler();

        var connectionString = _configuration.GetConnectionString("ChangeBlogDb");
        services.AddPostgresDataAccess(connectionString);
        services.AddHttpContextAccessor();
        services.AddScoped<IExternalUserInfoDao, TokenClaimsUserInfoDao>();

        services.AddMemoryCache();
        services.AddApplicationUseCases();

        var corsUrls = _configuration
            .GetSection("AppSettings:CorsUrls")
            .Get<string[]>();
        services.AddCors(options =>
        {
            options.AddPolicy(AllowManagementAppOrigin, builder =>
            {
                builder.WithOrigins(corsUrls)
                    .AllowAnyHeader()
                    .AllowAnyMethod();
            });
        });
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment()) app.UseDeveloperExceptionPage();

        app.AddSwagger();
        app.UseRouting();
        app.UseAuthentication();
        app.UseAuthorization();
        app.UseCors(AllowManagementAppOrigin);
        app.UseEndpoints(endpoints =>
            endpoints
                .MapControllers()
                .RequireAuthorization());
    }

    private static void ConfigureControllers(IServiceCollection services)
    {
        services
            .AddControllers(o => { o.Filters.Add(typeof(AuthorizationFilter)); })
            .ConfigureApiBehaviorOptions(o => o.InvalidModelStateResponseFactory = CustomErrorMessage);
    }

    private static ActionResult CustomErrorMessage(ActionContext context)
    {
        var firstError = context.ModelState
            .Where(x => x.Value is not null)
            .FirstOrDefault(x => x.Value.Errors.Count > 0)
            .Value?.Errors.FirstOrDefault()?
            .ErrorMessage ?? "Unknown";

        return new BadRequestObjectResult(DefaultResponse.Create(firstError));
    }
}