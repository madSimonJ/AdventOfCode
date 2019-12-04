using AdventOfCode.Common;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace AdventOfCode.Year2019
{
    public static class WireCalculator
    {
        private static readonly IDictionary<char, Func<int, Func<int, int, (int, int)>>> VectorLookup = new Dictionary<char, Func<int, Func<int, int, (int, int)>>>
        {
            {'R', z => (x, y) => (x + z, y) },
            {'L', z => (x, y) => (x - z, y) },
            {'U', z => (x, y) => (x, y + z) },
            {'D', z => (x, y) => (x, y - z) }
        };

        //private static (int, int) MakeCoordVector(char direction, int value) =>


        //public static IEnumerable<(int X, int Y)> GetWireCoords(IEnumerable<string> vectors, (int X, int Y) CurrLoc) =>
        //    vectors.Aggregate(new (int, int)[0],
        //            (acc, x) =>
        //                acc.Concat(
        //                    Enumerable.Range(0, int.Parse(x.Skip(1).ToString()))
        //                    .Select(y => VectorLookup[x[0]](y))
        //                    .Select(y => (CurrLoc.X + y.Item1, CurrLoc.Y + y.Item2))
        //            ).ToArray());


        public static IEnumerable<(int X, int Y)> ToCoords(this IEnumerable<Func<int, (int, int)>> deltas) =>
            deltas.GetEnumerator().ToCoords((0, 0));

        public static IEnumerable<(int DeltaX, int DeltaY)> GenerateDeltas(IEnumerable<string> instructions) =>
            instructions.SelectMany(
                    instructions.Select(
                            x => VectorLookup[x[0]](int.Parse(x[1].ToString())
                        )                            
                )

    }

    public class Day03
    {
        [Fact]
        public void Day03a_Test01()
        {
            var wire1 = new[] { "R8", "U5", "L5", "D3" };
            var wire2 = new[] { "U7", "R6", "D4", "L4" };
        }
    }
}
