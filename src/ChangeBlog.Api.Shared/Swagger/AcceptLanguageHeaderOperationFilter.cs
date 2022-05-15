using System.Collections.Generic;
using System.Linq;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace ChangeBlog.Api.Shared.Swagger;

public class AcceptLanguageHeaderOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        operation.Parameters ??= new List<OpenApiParameter>();

        var supportedLanguages = LocalizationOptions.SupportedLanguages
            .Select(x => new OpenApiString(x) as IOpenApiAny)
            .ToList();
        
        operation.Parameters.Add(new OpenApiParameter
        {
            Name = "Accept-Language",
            In = ParameterLocation.Header,
            Description = "Supported languages",
            Schema = new OpenApiSchema
            {   
                Default = new OpenApiString(LocalizationOptions.DefaultLanguage), 
                Type = "string",
                Enum = supportedLanguages
            },
            Required = false
        }) ;
    }
}