using NihFix.EfQueryCacheOptimizer;
using NihFix.EfQueryCacheOptimizer.Visitor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

// ReSharper disable UnusedMember.Global

// ReSharper disable once CheckNamespace
namespace System.Linq
{
    public static class CacheOptimizedQueryableExtensions
    {
        public static ICacheOptimizedQueryable<TSource> AsCacheOptimizedQueryable<TSource>(
            this IQueryable<TSource> originalQueryable)
        {
            return new CacheOptimizedQueryable<TSource>(originalQueryable);
        }

        public static IOrderedCacheOptimizedQueryable<TSource> AsOrderedCacheOptimizedQueryable<TSource>(
            this IOrderedQueryable<TSource> originalQueryable)
        {
            return new OrderedCacheOptimizedQueryable<TSource>(originalQueryable);
        }

        public static bool All<TSource>(this ICacheOptimizedQueryable<TSource> source,
            Expression<Func<TSource, bool>> predicate)
        {
            return DecorateMethod(source, predicate, (q, e) => q.All(e));
        }

        public static bool Any<TSource>(this ICacheOptimizedQueryable<TSource> source,
            Expression<Func<TSource, bool>> predicate)
        {
            return DecorateMethod(source, predicate, (q, e) => q.Any(e));
        }

        public static TSource First<TSource>(this ICacheOptimizedQueryable<TSource> source,
            Expression<Func<TSource, bool>> predicate)
        {
            return DecorateMethod(source, predicate, (q, e) => q.First(e));
        }

        public static TSource FirstOrDefault<TSource>(this ICacheOptimizedQueryable<TSource> source,
            Expression<Func<TSource, bool>> predicate)
        {
            return DecorateMethod(source, predicate, (q, e) => q.FirstOrDefault(e));
        }

        public static IQueryable<TResult> GroupBy<TSource, TKey, TElement, TResult>(
            this ICacheOptimizedQueryable<TSource> source, Expression<Func<TSource, TKey>> keySelector,
            Expression<Func<TSource, TElement>> elementSelector,
            Expression<Func<TKey, IEnumerable<TElement>, TResult>> resultSelector)
        {
            var optimizationConfig = source.OptimizationConfig;
            var keySelectorOptimised = OptimizeExpressionForCache(keySelector, optimizationConfig);
            var elementSelectorOptimised = OptimizeExpressionForCache(elementSelector, optimizationConfig);
            var resultSelectorOptimised = OptimizeExpressionForCache(resultSelector, optimizationConfig);
            return source.AsQueryable().GroupBy(keySelectorOptimised, elementSelectorOptimised, resultSelectorOptimised)
                .AsCacheOptimizedQueryable();
        }

        public static IQueryable<TResult> GroupBy<TSource, TKey, TResult>(this ICacheOptimizedQueryable<TSource> source,
            Expression<Func<TSource, TKey>> keySelector,
            Expression<Func<TKey, IEnumerable<TSource>, TResult>> resultSelector)
        {
            var optimizationConfig = source.OptimizationConfig;
            var keySelectorOptimised = OptimizeExpressionForCache(keySelector, optimizationConfig);
            var resultSelectorOptimised = OptimizeExpressionForCache(resultSelector, optimizationConfig);
            return source.AsQueryable().GroupBy(keySelectorOptimised, resultSelectorOptimised)
                .AsCacheOptimizedQueryable();
        }

        public static IQueryable<IGrouping<TKey, TElement>> GroupBy<TSource, TKey, TElement>(
            this ICacheOptimizedQueryable<TSource> source, Expression<Func<TSource, TKey>> keySelector,
            Expression<Func<TSource, TElement>> elementSelector)
        {
            var optimizationConfig = source.OptimizationConfig;
            var keySelectorOptimised = OptimizeExpressionForCache(keySelector, optimizationConfig);
            var elementSelectorOptimised = OptimizeExpressionForCache(elementSelector, optimizationConfig);
            return source.AsQueryable().GroupBy(keySelectorOptimised, elementSelectorOptimised)
                .AsCacheOptimizedQueryable();
        }

        public static IQueryable<IGrouping<TKey, TSource>> GroupBy<TSource, TKey>(
            this ICacheOptimizedQueryable<TSource> source, Expression<Func<TSource, TKey>> keySelector)
        {
            var optimizationConfig = source.OptimizationConfig;
            var keySelectorOptimised = OptimizeExpressionForCache(keySelector, optimizationConfig);
            return source.AsQueryable().GroupBy(keySelectorOptimised).AsCacheOptimizedQueryable();
        }

        //TODO: ADD WARNING FOR IQueryable And IEnumerable Outer
        public static IQueryable<TResult> GroupJoin<TOuter, TInner, TKey, TResult>(
            this ICacheOptimizedQueryable<TOuter> outer, ICacheOptimizedQueryable<TInner> inner,
            Expression<Func<TOuter, TKey>> outerKeySelector, Expression<Func<TInner, TKey>> innerKeySelector,
            Expression<Func<TOuter, IEnumerable<TInner>, TResult>> resultSelector)
        {
            var optimizationConfig = outer.OptimizationConfig;
            var outerSelectorOptimized = OptimizeExpressionForCache(outerKeySelector, optimizationConfig);
            var innerSelectorOptimized = OptimizeExpressionForCache(innerKeySelector, optimizationConfig);
            var resultSelectorOptimized = OptimizeExpressionForCache(resultSelector, optimizationConfig);
            return outer.AsQueryable().GroupJoin(inner.AsQueryable(), outerSelectorOptimized, innerSelectorOptimized,
                resultSelectorOptimized);
        }

        //TODO: ADD WARNING FOR IQueryable And IEnumerable Outer
        public static IQueryable<TResult> Join<TOuter, TInner, TKey, TResult>(
            this ICacheOptimizedQueryable<TOuter> outer, ICacheOptimizedQueryable<TInner> inner,
            Expression<Func<TOuter, TKey>> outerKeySelector, Expression<Func<TInner, TKey>> innerKeySelector,
            Expression<Func<TOuter, TInner, TResult>> resultSelector)
        {
            var optimizationConfig = outer.OptimizationConfig;
            var outerSelectorOptimized = OptimizeExpressionForCache(outerKeySelector, optimizationConfig);
            var innerSelectorOptimized = OptimizeExpressionForCache(innerKeySelector, optimizationConfig);
            var resultSelectorOptimized = OptimizeExpressionForCache(resultSelector, optimizationConfig);
            return outer.AsQueryable().Join(inner.AsQueryable(), outerSelectorOptimized, innerSelectorOptimized,
                resultSelectorOptimized);
        }

        public static IOrderedCacheOptimizedQueryable<TSource> OrderBy<TSource, TKey>(
            this ICacheOptimizedQueryable<TSource> source, Expression<Func<TSource, TKey>> keySelector)
        {
            return DecorateMethod(source, keySelector, (q, e) => q.OrderBy(e)).AsOrderedCacheOptimizedQueryable();
        }

        public static IOrderedCacheOptimizedQueryable<TSource> OrderByDescending<TSource, TKey>(
            this ICacheOptimizedQueryable<TSource> source, Expression<Func<TSource, TKey>> keySelector)
        {
            return DecorateMethod(source, keySelector, (q, e) => q.OrderByDescending(e))
                .AsOrderedCacheOptimizedQueryable();
        }

        public static ICacheOptimizedQueryable<TResult> Select<TSource, TResult>(
            this ICacheOptimizedQueryable<TSource> source, Expression<Func<TSource, TResult>> selector)
        {
            var optimizationConfig = source.OptimizationConfig;
            var optimizedExpression = OptimizeExpressionForCache(selector, optimizationConfig);
            return source.AsQueryable().Select(optimizedExpression).AsCacheOptimizedQueryable();
        }

        public static ICacheOptimizedQueryable<TResult> SelectMany<TSource, TResult>(
            this ICacheOptimizedQueryable<TSource> source, Expression<Func<TSource, IEnumerable<TResult>>> selector)
        {
            return DecorateMethod(source, selector, (q, e) => q.SelectMany(e)).AsCacheOptimizedQueryable();
        }

        public static ICacheOptimizedQueryable<TResult> SelectMany<TSource, TCollection, TResult>(
            this ICacheOptimizedQueryable<TSource> source,
            Expression<Func<TSource, IEnumerable<TCollection>>> collectionSelector,
            Expression<Func<TSource, TCollection, TResult>> resultSelector)
        {
            var optimizationConfig = source.OptimizationConfig;
            var collectionSelectorOptimized = OptimizeExpressionForCache(collectionSelector, optimizationConfig);
            var resultSelectorOptimized = OptimizeExpressionForCache(resultSelector, optimizationConfig);
            return source.AsQueryable().SelectMany(collectionSelectorOptimized, resultSelectorOptimized)
                .AsCacheOptimizedQueryable();
        }

        public static TSource Single<TSource>(this ICacheOptimizedQueryable<TSource> source,
            Expression<Func<TSource, bool>> predicate)
        {
            return DecorateMethod(source, predicate, (q, e) => q.Single(e));
        }

        public static TSource SingleOrDefault<TSource>(this ICacheOptimizedQueryable<TSource> source,
            Expression<Func<TSource, bool>> predicate)
        {
            return DecorateMethod(source, predicate, (q, e) => q.SingleOrDefault(e));
        }

        public static IOrderedCacheOptimizedQueryable<TSource> ThenBy<TSource, TKey>(
            this IOrderedCacheOptimizedQueryable<TSource> source, Expression<Func<TSource, TKey>> keySelector)
        {
            var optimizationConfig = source.OptimizationConfig;
            var keySelectorOptimized = OptimizeExpressionForCache(keySelector, optimizationConfig);
            return source.AsOrderedQueryable().ThenBy(keySelectorOptimized).AsOrderedCacheOptimizedQueryable();
        }

        public static IOrderedCacheOptimizedQueryable<TSource> ThenByDescending<TSource, TKey>(
            this IOrderedCacheOptimizedQueryable<TSource> source, Expression<Func<TSource, TKey>> keySelector)
        {
            var optimizationConfig = source.OptimizationConfig;
            var keySelectorOptimized = OptimizeExpressionForCache(keySelector, optimizationConfig);
            return source.AsOrderedQueryable().ThenByDescending(keySelectorOptimized)
                .AsOrderedCacheOptimizedQueryable();
        }

        public static ICacheOptimizedQueryable<TSource> Where<TSource>(this ICacheOptimizedQueryable<TSource> source,
            Expression<Func<TSource, bool>> predicate)
        {
            return DecorateMethod(source, predicate, (q, e) => q.Where(e)).AsCacheOptimizedQueryable();
        }

        public static ICacheOptimizedQueryable<TSource> Where<TSource>(this ICacheOptimizedQueryable<TSource> source,
            Expression<Func<TSource, int, bool>> predicate)
        {
            return DecorateMethod(source, predicate, (q, e) => q.Where(e)).AsCacheOptimizedQueryable();
        }

        private static T OptimizeExpressionForCache<T>(T expression, IOptimizationConfig optimizationConfig)
            where T : Expression
        {
            var visitor = new CacheOptimizedExpressionVisitor(optimizationConfig);
            var optimizedExpression = (T) visitor.Visit(expression);
            return optimizedExpression;
        }

        private static TOut DecorateMethod<TSource, TExpression, TOut>(this ICacheOptimizedQueryable<TSource> source,
            TExpression expression, Func<IQueryable<TSource>, TExpression, TOut> method) where TExpression : Expression
        {
            var optimizationConfig = source.OptimizationConfig;
            var optimizedExpression = OptimizeExpressionForCache(expression, optimizationConfig);
            var query = method(source.AsQueryable(), optimizedExpression);
            return query;
        }
    }
}