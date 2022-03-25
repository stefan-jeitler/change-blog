using System;

namespace ChangeBlog.Api.Shared.Swagger;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public class SwaggerControllerOrderAttribute : Attribute
{
    public SwaggerControllerOrderAttribute(ushort position)
    {
        Position = position;
    }

    public ushort Position { get; }
}