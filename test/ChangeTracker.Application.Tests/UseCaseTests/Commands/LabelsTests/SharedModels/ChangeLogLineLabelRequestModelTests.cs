using System;
using ChangeTracker.Application.UseCases.Command.Labels.SharedModels;
using FluentAssertions;
using Xunit;

namespace ChangeTracker.Application.Tests.UseCaseTests.Commands.LabelsTests.SharedModels
{
    public class ChangeLogLineLabelRequestModelTests
    {
        [Fact]
        public void Create_WithEmptyLineId_ArgumentException()
        {
            var lineId = Guid.Empty;

            Func<ChangeLogLineLabelRequestModel> act = () => new ChangeLogLineLabelRequestModel(lineId, "someLabel");

            act.Should().ThrowExactly<ArgumentException>();
        }

        [Fact]
        public void Create_WithNullLabel_ArgumentNullException()
        {
            Func<ChangeLogLineLabelRequestModel> act = () => new ChangeLogLineLabelRequestModel(Guid.NewGuid(), null);

            act.Should().ThrowExactly<ArgumentNullException>();
        }
    }
}