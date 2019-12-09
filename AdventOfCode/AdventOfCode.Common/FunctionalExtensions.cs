using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode.Common
{
    public static class FunctionalExtensions
    {
        public static TOutput Map<TInput, TOutput>(this TInput @this, Func<TInput, TOutput> f) =>
            f(@this);
        public static TOutput Match<TInput, TOutput>(this TInput @this, params (Func<TInput, bool> cond, Func<TInput, TOutput> trans)[] f) =>
            f.First(x => x.cond(@this)).trans(@this);

        private static IEnumerable<TOut> Select<TIn, TOut>(this IEnumerator<TIn> @this, Func<TIn, TIn, TOut> f, IEnumerable<TOut> acc) =>
            @this.Current
                .Map(x =>
                    @this.MoveNext()
                        ? Select(@this, f, acc.Concat(new[] { f(x, @this.Current) }))
                        : acc
        );

        public static IEnumerable<TOut> Select<TIn, TOut>(this IEnumerable<TIn> @this, Func<TIn, TIn, TOut> f) =>
            Select(@this.GetEnumerator(), f, Enumerable.Empty<TOut>());
    }
}
