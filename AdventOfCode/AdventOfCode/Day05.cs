using AdventOfCode.Common;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using AdventOfCode.Year2019.Common;

namespace AdventOfCode.Year2019
{
    public class Day05
    {
        [Fact]
        public void Day05a_Test01()
        {
            var input = new[] { 1002, 4, 3, 4, 33, 4, 4, 99 };
            var answer = OpcodeCalculator.Process(input);
            answer.Output.Single().Should().Be(99);
        }

        [Fact]
        public void Day05a()
        {
            var input = ContentLoader.Load(2019, 5, x => int.Parse(x), ",").ToArray();

            var output = OpcodeCalculator.Process(input, 1);
            output.Output.Should().NotBeEmpty();
        }
    }
}
