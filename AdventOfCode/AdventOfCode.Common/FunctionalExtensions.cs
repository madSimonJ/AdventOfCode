using System;

namespace AdventOfCode.Common
{
    public static class FunctionalExtensions
    {
        public static TOutput Map<TInput, TOutput>(this TInput @this, Func<TInput, TOutput> f) =>
            f(@this);
    }
}
