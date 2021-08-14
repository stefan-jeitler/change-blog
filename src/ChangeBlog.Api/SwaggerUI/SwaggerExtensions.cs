using System;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;

namespace ChangeBlog.Api.SwaggerUI
{
    public static class SwaggerExtensions
    {
        public static IServiceCollection AddSwagger(this IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo {Title = "ChangeBlog.Api", Version = "v1"});

                c.AddSecurityDefinition("ApiKey", new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.ApiKey,
                    In = ParameterLocation.Header,
                    Name = "X-API-KEY",
                    Description = "Api key authentication"
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "ApiKey"
                            }
                        },
                        Array.Empty<string>()
                    }
                });

                c.OrderActionsBy(api =>
                {
                    if (api.ActionDescriptor is not ControllerActionDescriptor descriptor)
                    {
                        return string.Empty;
                    }

                    var orderAttribute = descriptor
                        .EndpointMetadata
                        .OfType<SwaggerControllerOrderAttribute>()
                        .FirstOrDefault();

                    return orderAttribute is null
                        ? descriptor.ControllerName
                        : orderAttribute.Position.ToString();
                });
            });

            return services;
        }


        public static IApplicationBuilder AddSwagger(this IApplicationBuilder app)
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "ChangeBlog.Api v1");
                c.RoutePrefix = string.Empty;
            });

            return app;
        }
    }
}
