using ChangeBlog.Domain.Miscellaneous;

namespace ChangeBlog.Domain;

public static class Constants
{
    public static readonly Name[] SupportedCultures =
    {
        Name.Parse("de-AT"),
        Name.Parse("de-DE"),
        Name.Parse("de-CH"),
        Name.Parse("en-US"),
        Name.Parse("en-GB")
    };
}