using System;
using ChangeTracker.Api.DTOs;
using FluentAssertions;
using Xunit;

namespace ChangeTracker.Api.Tests
{
    public class ApiInfoTests
    {
        [Fact]
        public void ApiInfo_NameIsNull_ArgumentNullException()
        {
            Func<ApiInfo> act = () => new ApiInfo(null,
                "1.0.0",
                "net5.0",
                new[] {"/swagger"});

            act.Should().ThrowExactly<ArgumentNullException>();
        }

        [Fact]
        public void ApiInfo_NameIsEmpty_ArgumentException()
        {
            Func<ApiInfo> act = () => new ApiInfo(string.Empty,
                "1.0.0",
                "net5.0",
                new[] {"/swagger"});

            act.Should().ThrowExactly<ArgumentException>();
        }

        [Fact]
        public void ApiInfo_VersionIsNull_ArgumentNullException()
        {
            Func<ApiInfo> act = () => new ApiInfo("ChangeTracker.Api",
                null,
                "net5.0",
                new[] {"/swagger"});

            act.Should().ThrowExactly<ArgumentNullException>();
        }

        [Fact]
        public void ApiInfo_EnvironmentIsNull_ArgumentNullException()
        {
            Func<ApiInfo> act = () => new ApiInfo("ChangeTracker.Api",
                "0.0.0",
                null,
                new[] {"/swagger"});

            act.Should().ThrowExactly<ArgumentNullException>();
        }

        [Fact]
        public void ApiInfo_EnvironmentIsEmpty_ArgumentException()
        {
            Func<ApiInfo> act = () => new ApiInfo("ChangeTracker.Api",
                "0.0.0",
                string.Empty,
                new[] {"/swagger"});

            act.Should().ThrowExactly<ArgumentException>();
        }

        [Fact]
        public void ApiInfo_ImportantLinksAreNull_ArgumentNullException()
        {
            Func<ApiInfo> act = () => new ApiInfo("ChangeTracker.Api",
                "0.0.0",
                "net5.0",
                null);

            act.Should().ThrowExactly<ArgumentNullException>();
        }

        [Fact]
        public void ApiInfo_ImportantLinksContainsNullElement_ArgumentNullException()
        {
            Func<ApiInfo> act = () => new ApiInfo("ChangeTracker.Api",
                "0.0.0",
                "net5.0",
                new[] {(string) null});

            act.Should().ThrowExactly<ArgumentNullException>();
        }
    }
}