using System.Linq;
using JetBrains.Annotations;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace ChangeBlog.Api.Shared.Swagger;

[UsedImplicitly]
public class AcceptLanguageHeaderDocumentFilter : IDocumentFilter
{
    public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
    {
        var supportedLanguages = LocalizationOptions.SupportedLanguages
            .Select(x => new OpenApiString(x) as IOpenApiAny)
            .ToList();
        
        if (swaggerDoc is {Components: { }})
        {
            swaggerDoc.Components.Parameters.Add("AcceptLanguage", new OpenApiParameter
            {
                Name = "Accept-Language",
                In = ParameterLocation.Header,
                Description = "Defines which language should be used for response messages.",
                Schema = new OpenApiSchema
                {   
                    Default = new OpenApiString(LocalizationOptions.DefaultLanguage), 
                    Type = "string",
                    Enum = supportedLanguages,
                
                },
                Required = false
            });
        }
    }
}