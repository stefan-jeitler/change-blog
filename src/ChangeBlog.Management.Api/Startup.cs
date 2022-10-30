using ChangeBlog.Api.Shared;
using ChangeBlog.Api.Shared.Authentication;
using ChangeBlog.Api.Shared.Authorization;
using ChangeBlog.Api.Shared.UserInfo;
using ChangeBlog.Application.Boundaries.DataAccess.ExternalIdentity;
using ChangeBlog.DataAccess.Postgres;
using ChangeBlog.Management.Api.Authentication;
using ChangeBlog.Management.Api.Extensions;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
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
            .ConfigureApiBehaviorOptions(o => o.InvalidModelStateResponseFactory =
                ctx => ctx.ModelState.ToCustomErrorResponse());

        services.AddValidatorsFromAssemblyContaining<Startup>();
        services.AddFluentValidationAutoValidation();

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
}