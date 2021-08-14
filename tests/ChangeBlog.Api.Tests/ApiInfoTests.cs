using System;
using ChangeBlog.Api.DTOs;
using FluentAssertions;
using Xunit;

namespace ChangeBlog.Api.Tests
{
    public class ApiInfoTests
    {
        [Fact]
        public void ApiInfo_NameIsNull_ArgumentNullException()
        {
            Func<ApiInfo> act = () => new ApiInfo(null,
                "1.0.0",
                "net5.0");

            act.Should().ThrowExactly<ArgumentNullException>();
        }

        [Fact]
        public void ApiInfo_NameIsEmpty_ArgumentException()
        {
            Func<ApiInfo> act = () => new ApiInfo(string.Empty,
                "1.0.0",
                "net5.0");

            act.Should().ThrowExactly<ArgumentException>();
        }

        [Fact]
        public void ApiInfo_VersionIsNull_ArgumentNullException()
        {
            Func<ApiInfo> act = () => new ApiInfo("ChangeBlog.Api",
                null,
                "net5.0");

            act.Should().ThrowExactly<ArgumentNullException>();
        }

        [Fact]
        public void ApiInfo_EnvironmentIsNull_ArgumentNullException()
        {
            Func<ApiInfo> act = () => new ApiInfo("ChangeBlog.Api",
                "0.0.0",
                null);

            act.Should().ThrowExactly<ArgumentNullException>();
        }

        [Fact]
        public void ApiInfo_EnvironmentIsEmpty_ArgumentException()
        {
            Func<ApiInfo> act = () => new ApiInfo("ChangeBlog.Api",
                "0.0.0",
                string.Empty);

            act.Should().ThrowExactly<ArgumentException>();
        }
    }
}
