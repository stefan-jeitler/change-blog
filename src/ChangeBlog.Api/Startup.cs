using ChangeBlog.Api.Authentication;
using ChangeBlog.Api.Shared;
using ChangeBlog.Api.Shared.Authentication;
using ChangeBlog.Api.Shared.Authorization;
using ChangeBlog.Api.Shared.UserInfo;
using ChangeBlog.Api.Swagger;
using ChangeBlog.Application.Boundaries.DataAccess.ExternalIdentity;
using ChangeBlog.DataAccess.Postgres;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
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
        services
            .AddControllers(o => { o.Filters.Add(typeof(AuthorizationFilter)); })
            .ConfigureApiBehaviorOptions(o => o.InvalidModelStateResponseFactory =
                ctx => ctx.ModelState.ToCustomErrorResponse());

        services.AddValidatorsFromAssemblyContaining<Startup>();
        services.AddFluentValidationAutoValidation();

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
        if (env.IsDevelopment())
            app.UseDeveloperExceptionPage();

        app.AddSwagger();
        app.UseRouting();

        var localizationOptions = LocalizationOptions.Get();
        app.UseRequestLocalization(localizationOptions);

        app.UseAuthentication();
        app.UseAuthorization();

        app.UseCors(AllowManagementAppOrigin);
        app.UseEndpoints(endpoints =>
            endpoints
                .MapControllers()
                .RequireAuthorization());
    }
}