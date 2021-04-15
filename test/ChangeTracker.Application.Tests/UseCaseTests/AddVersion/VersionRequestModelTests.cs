﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChangeTracker.Application.UseCases.AddVersion;
using FluentAssertions;
using Xunit;

namespace ChangeTracker.Application.Tests.UseCaseTests.AddVersion
{
    public class VersionRequestModelTests
    {

        private Guid _testProjectId;
        private string _testVersion;

        public VersionRequestModelTests()
        {
            _testProjectId = Guid.Parse("f02cf1c7-d8a7-492f-b46d-a2ba916770d0");
            _testVersion = "1.2.3";
        }

        private VersionRequestModel CreateRequestModel() => new(_testProjectId, _testVersion);

        [Fact]
        public void Create_WithEmptyProjectId_ArgumentException()
        {
            _testProjectId = Guid.Empty;

            Func<VersionRequestModel> act = CreateRequestModel;

            act.Should().ThrowExactly<ArgumentException>();
        }

        [Fact]
        public void Create_WithNullVersion_ArgumentNullException()
        {
            _testVersion = null;

            Func<VersionRequestModel> act = CreateRequestModel;

            act.Should().ThrowExactly<ArgumentNullException>();
        }
    }
}
