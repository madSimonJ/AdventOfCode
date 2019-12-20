using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode.Common
{
    public static class FunctionalExtensions
    {
        public static TOutput Map<TInput, TOutput>(this TInput @this, Func<TInput, TOutput> f) =>
            f(@this);

        public static bool Any<T>(this IEnumerator<T> @this, Func<T, T, bool> f, T prev) =>
            @this.MoveNext()
                ? f(prev, @this.Current)
                    ? true
                    : Any(@this, f, @this.Current)
                : false;

        public static bool Any<T>(this IEnumerator<T> @this, Func<T, T, bool> f) =>
            @this.MoveNext()
            .Map(x => Any(@this, f, @this.Current));

        public static bool Any<T>(this IEnumerable<T> @this, Func<T, T, bool> f) =>
            @this.GetEnumerator().Any(f);

        public static bool All<T>(this IEnumerator<T> @this, Func<T, T, bool> f, T prev) =>
            @this.MoveNext()
                ? f(prev, @this.Current)
                    ? All(@this, f, @this.Current)
                    : false
                : true;

        public static bool All<T>(this IEnumerator<T> @this, Func<T, T, bool> f) =>
            @this.MoveNext()
            .Map(x => All(@this, f, @this.Current));

        public static bool All<T>(this IEnumerable<T> @this, Func<T, T, bool> f) =>
            @this.GetEnumerator().All(f);

        public static bool None<T>(this IEnumerator<T> @this, Func<T, T, T, bool> f, T prev1, T prev2) =>
            @this.MoveNext()
                ? f(prev1, prev2, @this.Current)
                    ? false
                    : None(@this, f, prev2, @this.Current)
                : true;

        public static bool None<T>(this IEnumerator<T> @this, Func<T, T, T, bool> f) =>
            @this.MoveNext()
            .Map(x => @this.Current)
            .Map(x =>
                (
                    p1: x,
                    p2: @this.MoveNext().Map(y => @this.Current)
                ))
            .Map(x => None(@this, f, x.p1, x.p2));

        public static bool None<T>(this IEnumerable<T> @this, Func<T, T, T, bool> f) =>
            @this.GetEnumerator().None(f);

        public static bool Validate<T>(this T @this, params Func<T, bool>[] predicates) =>
            predicates.All(x => x(@this));

        public static TOutput Match<TInput, TOutput>(this TInput @this, params (Func<TInput, bool> cond, Func<TInput, TOutput> trans)[] f) =>
            f.First(x => x.cond(@this)).trans(@this);

        public static TOutput Match<TInput, TOutput>(this TInput @this, params (TInput cond, Func<TInput, TOutput> trans)[] f) =>
            f.First(x => Comparer<TInput>.Default.Compare(x.cond, @this) == 0).trans(@this);

        public static TOutput Match<TInput, TOutput>(this TInput @this, params (TInput cond, TOutput trans)[] f) =>
            f.First(x => Comparer<TInput>.Default.Compare(x.cond, @this) == 0).trans;

        private static IEnumerable<TOut> Select<TIn, TOut>(this IEnumerator<TIn> @this, Func<TIn, TIn, TOut> f, IEnumerable<TOut> acc) =>
            @this.Current
                .Map(x =>
                    @this.MoveNext()
                        ? Select(@this, f, acc.Concat(new[] { f(x, @this.Current) }))
                        : acc
        );

        public static IEnumerable<TOut> Select<TIn, TOut>(this IEnumerable<TIn> @this, Func<TIn, TIn, TOut> f) =>
            Select(@this.GetEnumerator(), f, Enumerable.Empty<TOut>());

        public static T ApplyOperations<T>(this IEnumerator<T> @this, Func<T, T, T> op, T acc) =>
            @this.MoveNext()
                ? ApplyOperations(@this, op, op(acc, @this.Current))
                : acc;

        public static T ApplyOperations<T>(this IEnumerable<T> @this, Func<T, T, T> op) =>
            @this.GetEnumerator()
                .Map(x => x.MoveNext().Map(_ => x))
                .Map(x =>
                    ApplyOperations(x, op, x.Current)
                );
    }
}
