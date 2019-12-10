using AdventOfCode.Common;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace AdventOfCode.Year2019
{
    public static class UniversalOrbitMap
    {
        private static int CalculateIndirectOrbits(this IEnumerable<(string Body, string Orbits)> input, string curr = "COM", int noOrbits = 0) =>
            input.Where(x => x.Orbits == curr)
                .Sum(x => 1 + noOrbits + CalculateIndirectOrbits(input, x.Body, noOrbits + 1));

        public static int CalculateOrbitalChecksum(IEnumerable<string> input) =>
            input.Select(x => x.Split(")"))
            .Select(x => (x.Last(), x.First())).ToArray()
            .CalculateIndirectOrbits();

        private static int DistanceToSanta(this IEnumerable<(string Body, string Orbits)> input, string curr, string dest, HashSet<string> alreadyVisited = null, int noSteps = 0) =>
            input.Where(x => x.Body == curr).Select(x => x.Orbits)
            .Concat(
                input.Where(x => x.Orbits == curr).Select(x => x.Body)
            ).Where(x => !alreadyVisited.Contains(x))
            .Map(x =>
                x.Any()
                    ? x.Contains(dest)
                        ? noSteps + 1
                        : x.Min(y => DistanceToSanta(input, y, dest, alreadyVisited.Concat(new[] { curr }).ToHashSet(), noSteps + 1))
                    : int.MaxValue
            );

        public static int DistanceToSanta(IEnumerable<string> input) =>
            input.Select(x => x.Split(")"))
            .Select(x => (x.Last(), x.First())).ToArray()
            .Map(x => (
                You: x.Single(y => y.Item1 == "YOU").Item2,
                Santa: x.Single(y => y.Item1 == "SAN").Item2,
                Input: x
            ))
            .Map(y => 
                y.Input.DistanceToSanta(y.You, y.Santa, new HashSet<string>())
            );

    }

    public class Day06
    {
        [Fact]
        public void Day06a_Test01()
        {
            var input = new[]
            {
               "COM)B",
               "B)C",
               "C)D",
               "D)E",
               "E)F",
               "B)G",
               "G)H",
               "D)I",
               "E)J",
               "J)K",
               "K)L"
            };

            var answer = UniversalOrbitMap.CalculateOrbitalChecksum(input);
            answer.Should().Be(42);
        }

        [Fact]
        public void Day06a()
        {
            var input = ContentLoader.Load(2019, 6, x => x);
            var answer = UniversalOrbitMap.CalculateOrbitalChecksum(input);
            answer.Should().Be(186597);
        }

        [Fact]
        public void Day06b_Test01()
        {
            var input = new[]
            {
               "COM)B",
               "B)C",
               "C)D",
               "D)E",
               "E)F",
               "B)G",
               "G)H",
               "D)I",
               "E)J",
               "J)K",
               "K)L",
               "K)YOU",
               "I)SAN"
            };

            var answer = UniversalOrbitMap.DistanceToSanta(input);
            answer.Should().Be(4);
        }

        [Fact]
        public void Day06b()
        {
            var input = ContentLoader.Load(2019, 6, x => x);
            var answer = UniversalOrbitMap.DistanceToSanta(input);
            answer.Should().Be(412);
        }
    }
}
