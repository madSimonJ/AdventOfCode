using AdventOfCode.Common;
using AdventOfCode.Year2019.Common;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace AdventOfCode.Year2019
{
    public class Day02
    {
        [Fact]
        public void Day02a_test01()
        {
            var input = OpcodeCalculator.Process(new[] { 1, 9, 10, 3, 2, 3, 11, 0, 99, 30, 40, 50 });
            input.First.Should().Be(3500);
        }

        [Fact]
        public void Day02a_test02()
        {
            var input = OpcodeCalculator.Process(new[] { 1, 0, 0, 0, 99 });
            input.First.Should().Be(2);
        }

        [Fact]
        public void Day02a_test03()
        {
            var input = OpcodeCalculator.Process(new[] { 2, 3, 0, 3, 99 });
            input.First.Should().Be(2);
        }

        [Fact]
        public void Day02a_test04()
        {
            var input = OpcodeCalculator.Process(new[] { 2, 4, 4, 5, 99, 0 });
            input.First.Should().Be(2);
        }

        [Fact]
        public void Day02a_test05()
        {
            var input = OpcodeCalculator.Process(new[] { 1, 1, 1, 4, 99, 5, 6, 0, 99 });
            input.First.Should().Be(30);
        }

        [Fact]
        public void Day02a()
        {
            var input = ContentLoader.Load(2019, 2, x => int.Parse(x), ",").ToArray();
            input[1] = 12;
            input[2] = 2;
            var output = OpcodeCalculator.Process(input);
            output.First.Should().Be(3267740);
        }

        [Fact]
        public void Day02b()
        {
            var input = ContentLoader.Load(2019, 2, x => int.Parse(x), ",").ToArray();
            (int x, int y) answer = (0, 0);
            var keepLooping = true;
            for (var i = 0; i < input.Length; i++)
            {
                for (var j = 0; j < input.Length; j++)
                {
                    var thisInput = input.Clone() as int[];
                    thisInput[1] = i;
                    thisInput[2] = j;
                    var output = OpcodeCalculator.Process(thisInput).First;
                    if (output == 19690720)
                    {
                        answer = (i, j);
                        break;
                    }
                        
                }

                if (!keepLooping) break;
            }

            ((answer.x * 100) + answer.y).Should().Be(7870);

        }
    }
}
