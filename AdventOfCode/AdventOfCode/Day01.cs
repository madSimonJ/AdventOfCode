using AdventOfCode.Common;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace AdventOfCode.Year2019
{
    public static class MassCalculator
    {
        public static int CalculateFuelRequired(int input) =>
            input.Map(x => x / 3)
                .Map(x => x - 2);

        public static int CalculateFuelRequired(IEnumerable<int> input) =>
            input.Sum(CalculateFuelRequired);

        public static int CalculateFuelRequiredIncludingFuelWeight(int input, int acc = 0) =>
            CalculateFuelRequired(input)
            .Map(x =>
                x <= 0
                    ? acc
                    : CalculateFuelRequiredIncludingFuelWeight(x, acc + x)
            );

        public static int CalculateFuelRequiredIncludingFuelWeight(IEnumerable<int> input) =>
            input.Sum(x => CalculateFuelRequiredIncludingFuelWeight(x));
    }

    public class Day01
    {
        [Fact]
        public void Day01a_test01()
        {
            MassCalculator.CalculateFuelRequired(12).Should().Be(2);
        }

        [Fact]
        public void Day01a_test02()
        {
            MassCalculator.CalculateFuelRequired(14).Should().Be(2);
        }

        [Fact]
        public void Day01a_test03()
        {
            MassCalculator.CalculateFuelRequired(1969).Should().Be(654);
        }

        [Fact]
        public void Day01a_test04()
        {
            MassCalculator.CalculateFuelRequired(100756).Should().Be(33583);
        }

        [Fact]
        public void Day01a()
        {
            var input = ContentLoader.Load(2019, 1, x => int.Parse(x));
            MassCalculator.CalculateFuelRequired(input).Should().Be(3327415);
        }

        [Fact]
        public void Day01b_test01()
        {
            MassCalculator.CalculateFuelRequiredIncludingFuelWeight(1969).Should().Be(966);
        }

        [Fact]
        public void Day01b_test02()
        {
            MassCalculator.CalculateFuelRequiredIncludingFuelWeight(100756).Should().Be(50346);
        }

        [Fact]
        public void Day01b()
        {
            var input = ContentLoader.Load(2019, 1, x => int.Parse(x));
            MassCalculator.CalculateFuelRequiredIncludingFuelWeight(input).Should().Be(4988257);
        }
    }
}
