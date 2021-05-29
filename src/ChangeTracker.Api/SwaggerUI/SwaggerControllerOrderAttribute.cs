using System;

namespace ChangeTracker.Api.SwaggerUI
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class SwaggerControllerOrderAttribute : Attribute
    {
        public SwaggerControllerOrderAttribute(ushort position)
        {
            Position = position;
        }

        public ushort Position { get; }
    }
}