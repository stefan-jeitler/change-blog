using System;
using System.Collections.Generic;
using System.Linq;
using ChangeBlog.Application.Extensions;
using FluentAssertions;
using Xunit;

namespace ChangeBlog.Application.Tests.ExtensionsTests
{
    public class EnumerableExtensionsTests
    {
        [Fact]
        public void DistinctBy_WithNullSource_EagerArgumentNullException()
        {
            Func<IEnumerable<int>> act = () => ((IEnumerable<int>) null).DistinctBy(x => x);

            act.Should().ThrowExactly<ArgumentNullException>();
        }

        [Fact]
        public void DistinctBy_WithNullSelector_EagerArgumentNullException()
        {
            var sequence = Enumerable.Range(0, 5);

            Func<IEnumerable<int>> act = () => sequence.DistinctBy((Func<int, bool>) null);

            act.Should().ThrowExactly<ArgumentNullException>();
        }

        [Fact]
        public void DistinctBy_StringWithTwoIdenticalEntries_DuplicatesRemoved()
        {
            var sequence = Enumerable.Range(0, 2).Select(_ => "ab");

            var unique = sequence.DistinctBy(x => x).ToList();

            unique.Should().ContainSingle(x => x == "ab");
            unique.Should().HaveCount(1);
        }
    }
}
