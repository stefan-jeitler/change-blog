using System.Collections.Generic;
using JetBrains.Annotations;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace ChangeBlog.Api.Shared.Swagger;

[UsedImplicitly]
public class AcceptLanguageHeaderOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        operation.Parameters ??= new List<OpenApiParameter>();

        operation.Parameters.Add(
            new OpenApiParameter
            {
                Reference = new OpenApiReference
                {
                    Id = "components/parameters/AcceptLanguage", 
                    ExternalResource = string.Empty,
                    Type = ReferenceType.Header,
                }
            });
    }
}