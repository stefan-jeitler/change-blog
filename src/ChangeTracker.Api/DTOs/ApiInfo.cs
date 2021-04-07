using System;
using System.Linq;

// ReSharper disable ConvertIfStatementToSwitchStatement

namespace ChangeTracker.Api.DTOs
{
    public class ApiInfo
    {
        public ApiInfo(string name, string version, string environment, string[] importantLinks)
        {
            if (name is null)
                throw new ArgumentNullException(nameof(name));

            if (name == string.Empty)
                throw new ArgumentException("name cannot be empty.");

            Name = name;

            if (version is null)
                throw new ArgumentNullException(nameof(version));

            if (version == string.Empty)
                throw new ArgumentException("version cannot be empty");

            Version = version;

            if (environment is null)
                throw new ArgumentNullException(nameof(environment));

            if (environment == string.Empty)
                throw new ArgumentException("environment cannot be empty");

            Environment = environment;

            if (importantLinks is null ||
                importantLinks.Any(x => x is null))
                throw new ArgumentNullException(nameof(importantLinks));

            ImportantLinks = importantLinks;
        }

        public string Name { get; }
        public string Version { get; }
        public string Environment { get; }
        public string[] ImportantLinks { get; }
    }
}