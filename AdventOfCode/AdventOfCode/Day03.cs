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
        private static readonly IDictionary<char, Func<int, int, (int, int)>> VectorLookup = new Dictionary<char, Func<int, int, (int, int)>>
        {
            {'R', (x, y) => (x + 1, y) },
            {'L', (x, y) => (x - 1, y) },
            {'U', (x, y) => (x, y + 1) },
            {'D', (x, y) => (x, y - 1) }
        };

        public static IEnumerable<(int X, int Y)> GetCoordsFromDeltas(this IEnumerator<Func<int, int, (int, int)>> @this, IEnumerable<(int X, int Y)> acc = null) =>
            (acc ?? @this.MoveNext().Map(_ => new[] { (0, 0) }))
            .Map(a =>
                (
                    last: a.Last(),
                    currentFunc: @this.Current,
                    a: a
                ))
                .Map(x =>
                   (
                        a: x.a,
                        UpdatedCoord: x.currentFunc(x.last.X, x.last.Y)

                    )
                )
                .Map(x =>
                    @this.MoveNext()
                        ? GetCoordsFromDeltas(@this, x.a.Concat(new[] { x.UpdatedCoord }))
                        : x.a.Concat(new[] { x.UpdatedCoord })
                );


        public static IEnumerable<(int X, int Y)> LayWire(IEnumerable<string> input) =>
            input.Select(x => (Direction: x[0], NoSteps: int.Parse(x.Substring(1))))
            .SelectMany(x => Enumerable.Repeat(VectorLookup[x.Direction], x.NoSteps))
            .GetEnumerator()
            .GetCoordsFromDeltas();

        public static int GetIntersections(IEnumerable<(int X, int Y)> a, IEnumerable<(int X, int Y)> b) =>
            a.Intersect(
                    b
                )
            .Select(x => Math.Abs(0 - x.X) + Math.Abs(0 - x.Y))
            .OrderBy(x => x)
            .Skip(1)
            .First();
    }

    public class WireVector
    {
        public char Direction { get; set; }
        public int NumberOfMoves { get; set; }
        public int StartX { get; set; }
        public int StartY { get; set; }
        public int StepsTaken { get; set; }
    }

    public static class WireCalculator2
    {
        public static (int X, int Y, int StepsSoFar) RunVector(this WireVector w) =>
            w.Direction.Match(
                    (x => x == 'R', _ => (w.StartX + w.NumberOfMoves, w.StartY, w.StepsTaken + w.NumberOfMoves)),
                    (x => x == 'L', _ => (w.StartX - w.NumberOfMoves, w.StartY, w.StepsTaken + w.NumberOfMoves)),
                    (x => x == 'U', _ => (w.StartX, w.StartY + w.NumberOfMoves, w.StepsTaken + w.NumberOfMoves)),
                    (x => x == 'D', _ => (w.StartX, w.StartY - w.NumberOfMoves, w.StepsTaken + w.NumberOfMoves))
                );

        public static IEnumerable<WireVector> LayWire(IEnumerable<string> input) =>
            input.Aggregate(
                Enumerable.Empty<WireVector>(),
                (acc, item) =>
                    acc.Match(
                            (x => x.Any(), x => x.Last().Map(RunVector)),
                            (_ => true, _ => (X: 0, Y: 0, StepsSoFar: 0))
                        )
                .Map(x =>
                    acc.Concat(
                        new[]
                        {
                            new WireVector
                            {
                                Direction = item[0],
                                NumberOfMoves = int.Parse(item.Substring(1)),
                                StartX = x.X,
                                StartY = x.Y,
                                StepsTaken = x.StepsSoFar
                            }
                        }
                    )
                )
            );

        public static bool IsVertical(this WireVector w) =>
            w.Direction == 'U' || w.Direction == 'D';

        public static bool IsHorizontal(this WireVector w) =>
            w.Direction == 'L' || w.Direction == 'R';

        public static bool IsBetween(this int @this, int a, int b) =>
            @this > Math.Min(a, b) && @this < Math.Max(a, b);

        public static bool IsIntersection(WireVector w1, WireVector w2) =>
            ((w1.IsVertical() && w2.IsHorizontal()) ||
            (w1.IsHorizontal() && w2.IsVertical())) &&
            (
                (w1.IsVertical() && w1.StartX.IsBetween(w2.StartX, w2.RunVector().X) && w2.StartY.IsBetween(w1.StartY, w1.RunVector().Y)) ||
                (w1.IsHorizontal() && w1.StartY.IsBetween(w2.StartY, w2.RunVector().Y) && w2.StartX.IsBetween(w1.StartX, w1.RunVector().X))
            );

        private static int ManhattanDistance(this WireVector @this, int x2, int y2) =>
            Math.Abs(@this.StartX - x2) + Math.Abs(@this.StartY - y2);

        public static (int X, int Y, int NumberOfSteps) GetIntersection(WireVector w1, WireVector w2) =>
            w1.IsVertical()
                ? (w1.StartX, w2.StartY, w1.StepsTaken + w1.ManhattanDistance(w1.StartX, w2.StartY))
                : (w2.StartX, w1.StartY, w1.StepsTaken + w1.ManhattanDistance(w2.StartX, w1.StartY));

        public static IEnumerable<(int X, int Y, int NumberOfSteps)> GetIntersections(IEnumerable<WireVector> a, IEnumerable<WireVector> b)
        {
            var a1 = a.Select(w1 => (ThisVector: w1, PossibleCrosses: b.Where(w2 => IsIntersection(w1, w2)).ToArray())).ToArray();
            return a1.SelectMany(x => x.PossibleCrosses.Select(y => GetIntersection(x.ThisVector, y)).ToArray());
        }

        public static int GetNearestIntersection(IEnumerable<(int X, int Y, int NumberOfSteps)> intersections) =>
            intersections.Select(x => Math.Abs(x.X) + Math.Abs(x.Y))
            .OrderBy(x => x)
            .First();

        public static int GetFewestSteps(IEnumerable<(int X, int Y, int NumberOfSteps)> w1, IEnumerable<(int X, int Y, int NumberOfSteps)> w2) =>
            w1.Join(w2, x => (x.X, x.Y), x => (x.X, x.Y), (a, b) => (X: a.X, Y: a.Y, Steps: a.NumberOfSteps + b.NumberOfSteps))
                .OrderBy(x => x.Steps).First().Steps;
    }

    public class Day03
    {
        [Fact]
        public void Day03a_Test01()
        {
            var wire1 = WireCalculator2.LayWire(new[] { "R8", "U5", "L5", "D3" });
            var wire2 = WireCalculator2.LayWire(new[] { "U7", "R6", "D4", "L4" });

            var answer = WireCalculator2.GetIntersections(wire1, wire2);
            var answer2 = WireCalculator2.GetNearestIntersection(answer);
            answer2.Should().Be(6);
        }

        [Fact]
        public void Day03a_Test02()
        {
            var wire1 = WireCalculator2.LayWire(new[] { "R75", "D30", "R83", "U83", "L12", "D49", "R71", "U7", "L72" });
            var wire2 = WireCalculator2.LayWire(new[] { "U62", "R66", "U55", "R34", "D71", "R55", "D58", "R83" });
            var answer = WireCalculator2.GetIntersections(wire1, wire2);
            var answer2 = WireCalculator2.GetNearestIntersection(answer);
            answer2.Should().Be(159);
        }

        [Fact]
        public void Day03a_Test03()
        {
            var wire1 = WireCalculator2.LayWire(new[] { "R98", "U47", "R26", "D63", "R33", "U87", "L62", "D20", "R33", "U53", "R51"});
            var wire2 = WireCalculator2.LayWire(new[] { "U98", "R91", "D20", "R16", "D67", "R40", "U7", "R15", "U6", "R7" });

            var answer = WireCalculator2.GetIntersections(wire1, wire2);
            var answer2 = WireCalculator2.GetNearestIntersection(answer);
            answer2.Should().Be(135);
        }

        [Fact]
        public void Day03a()
        {
            var input = ContentLoader.Load(2019, 3, x => x.Split(",")).ToArray();
            var wire1 = WireCalculator2.LayWire(input[0]);
            var wire2 = WireCalculator2.LayWire(input[1]);

            var answer = WireCalculator2.GetIntersections(wire1, wire2);
            var answer2 = WireCalculator2.GetNearestIntersection(answer);
            answer2.Should().Be(386);
        }


        [Fact]
        public void Day03b_Test01()
        {
            var wire1 = WireCalculator2.LayWire(new[] { "R8", "U5", "L5", "D3" });
            var wire2 = WireCalculator2.LayWire(new[] { "U7", "R6", "D4", "L4" });

            var answer = WireCalculator2.GetIntersections(wire1, wire2);
            var answer2 = WireCalculator2.GetIntersections(wire2, wire1);

            var answer3 = WireCalculator2.GetFewestSteps(answer, answer2);
            answer3.Should().Be(30);
            
        }

        [Fact]
        public void Day03b_Test02()
        {
            var wire1 = WireCalculator2.LayWire(new[] { "R75", "D30", "R83", "U83", "L12", "D49", "R71", "U7", "L72" });
            var wire2 = WireCalculator2.LayWire(new[] { "U62", "R66", "U55", "R34", "D71", "R55", "D58", "R83" });

            var answer = WireCalculator2.GetIntersections(wire1, wire2);
            var answer2 = WireCalculator2.GetIntersections(wire2, wire1);

            var answer3 = WireCalculator2.GetFewestSteps(answer, answer2);
            answer3.Should().Be(610);
        }

        [Fact]
        public void Day03b_Test03()
        {
            var wire1 = WireCalculator2.LayWire(new[] { "R98", "U47", "R26", "D63", "R33", "U87", "L62", "D20", "R33", "U53", "R51" });
            var wire2 = WireCalculator2.LayWire(new[] { "U98", "R91", "D20", "R16", "D67", "R40", "U7", "R15", "U6", "R7" });

            var answer = WireCalculator2.GetIntersections(wire1, wire2);
            var answer2 = WireCalculator2.GetIntersections(wire2, wire1);

            var answer3 = WireCalculator2.GetFewestSteps(answer, answer2);
            answer3.Should().Be(410);
        }

        [Fact]
        public void Day03b()
        {
            var input = ContentLoader.Load(2019, 3, x => x.Split(",")).ToArray();
            var wire1 = WireCalculator2.LayWire(input[0]);
            var wire2 = WireCalculator2.LayWire(input[1]);

            var answer = WireCalculator2.GetIntersections(wire1, wire2);
            var answer2 = WireCalculator2.GetIntersections(wire2, wire1);

            var answer3 = WireCalculator2.GetFewestSteps(answer, answer2);
            answer3.Should().Be(6484);
        }
    }
}
