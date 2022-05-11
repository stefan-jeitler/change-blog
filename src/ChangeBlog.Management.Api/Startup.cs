using System.Linq;
using ChangeBlog.Api.Localization.Resources;
using ChangeBlog.Api.Shared;
using ChangeBlog.Api.Shared.Authentication;
using ChangeBlog.Api.Shared.Authorization;
using ChangeBlog.Api.Shared.DTOs;
using ChangeBlog.Api.Shared.UserInfo;
using ChangeBlog.Application.Boundaries.DataAccess.ExternalIdentity;
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

namespace ChangeBlog.Management.Api;

public class Startup
{
    private readonly IConfiguration _configuration;

    public Startup(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public void ConfigureServices(IServiceCollection services)
    {
        var settings = _configuration
            .GetSection("Authentication:AzureAdB2C")
            .Get<MicrosoftIdentityAuthenticationSettings>();

        services
            .AddAuthenticationServices()
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddAppAuthentication(settings);

        services.AddPermissionHandler();

        services
            .AddControllers(o => { o.Filters.Add(typeof(AuthorizationFilter)); })
            .ConfigureApiBehaviorOptions(o => o.InvalidModelStateResponseFactory = CustomErrorMessage);

        services.AddSpaStaticFiles(configuration => { configuration.RootPath = "ClientApp/dist"; });

        services
            .AddSwagger()
            .AddApplicationUseCases();

        var connectionString = _configuration.GetConnectionString("ChangeBlogDb");
        services.AddPostgresDataAccess(connectionString);
        services.AddHttpContextAccessor();
        services.AddScoped<IExternalUserInfoDao, TokenClaimsUserInfoDao>();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
            app.UseDeveloperExceptionPage();

        app.AddSwagger();

        app.UseStaticFiles();

        app.UseSpaStaticFiles(new StaticFileOptions
        {
            OnPrepareResponse = context => { context.Context.Response.Headers.Add("Cache-Control", "no-cache"); }
        });

        app.UseRouting();

        var localizationOptions = LocalizationOptions.Get();
        app.UseRequestLocalization(localizationOptions);

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
                spa.UseAngularCliServer("start");
        });
    }

    private static ActionResult CustomErrorMessage(ActionContext context)
    {
        var firstError = context.ModelState
            .Where(x => x.Value is not null)
            .FirstOrDefault(x => x.Value.Errors.Count > 0)
            .Value?.Errors.FirstOrDefault()?
            .ErrorMessage ?? ChangeBlogStrings.UnknownError;

        return new BadRequestObjectResult(ErrorResponse.Create(firstError));
    }
}