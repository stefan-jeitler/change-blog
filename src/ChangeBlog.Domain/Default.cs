using System;
using ChangeBlog.Domain.Miscellaneous;

namespace ChangeBlog.Domain;

public static class Default
{
    public static readonly Guid VersioningSchemeId = Guid.Parse("4091b948-9bc5-43ee-9f98-df3d27853565");
    
    public static readonly Name Timezone = Name.Parse("Etc/UTC");

    public static readonly Name Culture = Name.Parse("en-US"); 
}