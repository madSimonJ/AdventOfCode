using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AdventOfCode.Common
{
    public static class ContentLoader
    {
        public static IEnumerable<string> Load(int year, int day, string splitter = null) =>
            File.ReadAllText($".\\Content\\{year}\\Day{day}.txt")
            .Split(splitter ?? Environment.NewLine);

        public static IEnumerable<T> Load<T>(int year, int day, Func<string, T> converter, string splitter = null) =>
            Load(year, day, splitter)
            .Select(converter);
    }
}
