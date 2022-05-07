using System.Linq;
using ChangeBlog.Domain;
using Microsoft.AspNetCore.Builder;

namespace ChangeBlog.Api.Shared;

public static class LocalizationOptions
{
    public static RequestLocalizationOptions Get()
    {
        var supportedCultures = Constants.SupportedCultures
            .Select(x => x.Value.Split("-").FirstOrDefault())
            .Where(x => x is not null)
            .Distinct()
            .ToArray();
        
        var defaultCulture = Default.Culture.Value.Split("-").First();

        return new RequestLocalizationOptions()
            .SetDefaultCulture(defaultCulture)
            .AddSupportedCultures(supportedCultures)
            .AddSupportedUICultures(supportedCultures);
    }
}