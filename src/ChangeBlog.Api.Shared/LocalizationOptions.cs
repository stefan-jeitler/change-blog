using System.Linq;
using ChangeBlog.Domain;
using Microsoft.AspNetCore.Builder;

namespace ChangeBlog.Api.Shared;

public static class LocalizationOptions
{
    public static readonly string[] SupportedLanguages = Constants.SupportedCultures
        .Select(x => x.Value.Split("-").FirstOrDefault())
        .Where(x => x is not null)
        .Distinct()
        .ToArray(); 
    
    public static readonly string DefaultLanguage = Default.Culture.Value.Split("-").First(); 
    
    public static RequestLocalizationOptions Get()
    {
        return new RequestLocalizationOptions()
            .SetDefaultCulture(DefaultLanguage)
            .AddSupportedUICultures(SupportedLanguages);
    }
}