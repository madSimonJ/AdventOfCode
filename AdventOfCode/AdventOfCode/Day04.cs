using AdventOfCode.Common;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace AdventOfCode.Year2019
{
    public static class SecureContainer 
    {
        public static bool ValidateNumber(int input) =>
            input.ToString()
            .Validate(
                x => x.Any((a, b) => a == b),
                x => x.All((a, b) => b >= a)
            );

        private static bool ContainsGroupOfTwo(this IEnumerator<char> @this, char prev = '0', int groupSize = 0) =>
            @this.MoveNext()
                ? @this.Current == prev
                    ? ContainsGroupOfTwo(@this, @this.Current, groupSize + 1)
                    : groupSize == 2
                        ? true
                        : ContainsGroupOfTwo(@this, @this.Current, 1)
                : groupSize == 2;

        private static bool ContainsGroupOfTwo(this IEnumerable<char> @this) =>
            @this.GetEnumerator().ContainsGroupOfTwo();

        public static bool ValidateNumber2(int input) =>
            input.ToString()
            .Validate(
                x => x.All((a, b) => b >= a),
                x => x.ContainsGroupOfTwo()
            );
    }

    public class Day04
    {
        [Fact]
        public void Day04a_Test01()
        {
            SecureContainer.ValidateNumber(111111).Should().BeTrue();
        }

        [Fact]
        public void Day04a_Test02()
        {
            SecureContainer.ValidateNumber(223450).Should().BeFalse();
        }

        [Fact]
        public void Day04a_Test03()
        {
            SecureContainer.ValidateNumber(123789).Should().BeFalse();
        }

        [Fact]
        public void Day04a()
        {
            var answer = Enumerable.Range(206938, 679128 - 206938)
                .Count(x => SecureContainer.ValidateNumber(x));
            answer.Should().Be(1653);
        }

        [Fact]
        public void Day04b_Test01()
        {
            SecureContainer.ValidateNumber(112233).Should().BeTrue();
        }

        [Fact]
        public void Day04b_Test02()
        {
            SecureContainer.ValidateNumber2(123444).Should().BeFalse();
        }

        [Fact]
        public void Day04b_Test03()
        {
            SecureContainer.ValidateNumber2(111122).Should().BeTrue();
        }

        [Fact]
        public void Day04b()
        {
            var answer = Enumerable.Range(206938, 679128 - 206938)
                .Count(x => SecureContainer.ValidateNumber2(x));
            answer.Should().BeLessThan(0);
        }
    }
}
