﻿using NihFix.EfQueryCacheOptimizer;
using NihFix.EfQueryCacheOptimizer.Visitor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace System.Linq
{
    public static class CacheOptimizedQueriableExtentions
    {
        public static ICacheOptimizedQueryable<TSource> AsCacheOptimizedQueriable<TSource>(this IQueryable<TSource> originalQueriable)
        {
            return new CacheOptimizedQueryable<TSource>(originalQueriable);
        }
        public static IOrderedCacheOptimizedQueryable<TSource> AsOrderedCacheOptimizedQueriable<TSource>(this IOrderedQueryable<TSource> originalQueriable)
        {
            return new OrderedCacheOptimizedQueryable<TSource>(originalQueriable);
        }

        public static bool All<TSource>(this ICacheOptimizedQueryable<TSource> source, Expression<Func<TSource, bool>> predicate)
        {
            return DecorateMethod(source, predicate, (q, e) => q.All(e));
        }

        public static bool Any<TSource>(this ICacheOptimizedQueryable<TSource> source, Expression<Func<TSource, bool>> predicate)
        {
            return DecorateMethod(source, predicate, (q, e) => q.Any(e));
        }

        public static TSource First<TSource>(this ICacheOptimizedQueryable<TSource> source, Expression<Func<TSource, bool>> predicate) {
            return DecorateMethod(source, predicate, (q, e) => q.First(e));
        }

        public static TSource FirstOrDefault<TSource>(this ICacheOptimizedQueryable<TSource> source, Expression<Func<TSource, bool>> predicate) {
            return DecorateMethod(source, predicate, (q, e) => q.FirstOrDefault(e));
        }

        public static IQueryable<TResult> GroupBy<TSource, TKey, TElement, TResult>(this ICacheOptimizedQueryable<TSource> source, Expression<Func<TSource, TKey>> keySelector, Expression<Func<TSource, TElement>> elementSelector, Expression<Func<TKey, IEnumerable<TElement>, TResult>> resultSelector) {
            var keySelectorOptimised = OptimizeExpressionForCache(keySelector);
            var elementSelectorOptimised = OptimizeExpressionForCache(elementSelector);
            var resultSelectorOptimised = OptimizeExpressionForCache(resultSelector);
            return source.AsQueryable().GroupBy(keySelectorOptimised, elementSelectorOptimised, resultSelectorOptimised).AsCacheOptimizedQueriable();
        }

        public static IQueryable<TResult> GroupBy<TSource, TKey, TResult>(this ICacheOptimizedQueryable<TSource> source, Expression<Func<TSource, TKey>> keySelector, Expression<Func<TKey, IEnumerable<TSource>, TResult>> resultSelector) {
            var keySelectorOptimised = OptimizeExpressionForCache(keySelector);
            var resultSelectorOptimised = OptimizeExpressionForCache(resultSelector);
            return source.AsQueryable().GroupBy(keySelectorOptimised, resultSelectorOptimised).AsCacheOptimizedQueriable();
        }

        public static IQueryable<IGrouping<TKey, TElement>> GroupBy<TSource, TKey, TElement>(this ICacheOptimizedQueryable<TSource> source, Expression<Func<TSource, TKey>> keySelector, Expression<Func<TSource, TElement>> elementSelector) {
            var keySelectorOptimised = OptimizeExpressionForCache(keySelector);
            var elementSelectorOptimised = OptimizeExpressionForCache(elementSelector);
            return source.AsQueryable().GroupBy(keySelectorOptimised, elementSelectorOptimised).AsCacheOptimizedQueriable();
        }

        public static IQueryable<IGrouping<TKey, TSource>> GroupBy<TSource, TKey>(this ICacheOptimizedQueryable<TSource> source, Expression<Func<TSource, TKey>> keySelector) {
            var keySelectorOptimised = OptimizeExpressionForCache(keySelector);
            return source.AsQueryable().GroupBy(keySelectorOptimised).AsCacheOptimizedQueriable();
        }

        //TODO: ADD WARNING FOR IQueriable And IEnumerable Outer
        public static IQueryable<TResult> GroupJoin<TOuter, TInner, TKey, TResult>(this ICacheOptimizedQueryable<TOuter> outer, ICacheOptimizedQueryable<TInner> inner, Expression<Func<TOuter, TKey>> outerKeySelector, Expression<Func<TInner, TKey>> innerKeySelector, Expression<Func<TOuter, IEnumerable<TInner>, TResult>> resultSelector) {
            var outerSelecterOptimized = OptimizeExpressionForCache(outerKeySelector);
            var innerSelecterOptimized = OptimizeExpressionForCache(innerKeySelector);
            var resultSelecterOptimized = OptimizeExpressionForCache(resultSelector);
            return outer.AsQueryable().GroupJoin(inner.AsQueryable(), outerSelecterOptimized, innerSelecterOptimized, resultSelecterOptimized);
        }

        //TODO: ADD WARNING FOR IQueriable And IEnumerable Outer
        public static IQueryable<TResult> Join<TOuter, TInner, TKey, TResult>(this ICacheOptimizedQueryable<TOuter> outer, ICacheOptimizedQueryable<TInner> inner, Expression<Func<TOuter, TKey>> outerKeySelector, Expression<Func<TInner, TKey>> innerKeySelector, Expression<Func<TOuter, TInner, TResult>> resultSelector) {
            var outerSelecterOptimized = OptimizeExpressionForCache(outerKeySelector);
            var innerSelecterOptimized = OptimizeExpressionForCache(innerKeySelector);
            var resultSelecterOptimized = OptimizeExpressionForCache(resultSelector);
            return outer.AsQueryable().Join(inner.AsQueryable(), outerSelecterOptimized, innerSelecterOptimized, resultSelecterOptimized);
        }

        public static IOrderedCacheOptimizedQueryable<TSource> OrderBy<TSource, TKey>(this ICacheOptimizedQueryable<TSource> source, Expression<Func<TSource, TKey>> keySelector) {
            return DecorateMethod(source, keySelector, (q, e) => q.OrderBy(e)).AsOrderedCacheOptimizedQueriable();
        }

        public static IOrderedCacheOptimizedQueryable<TSource> OrderByDescending<TSource, TKey>(this ICacheOptimizedQueryable<TSource> source, Expression<Func<TSource, TKey>> keySelector) {
            return DecorateMethod(source, keySelector, (q, e) => q.OrderByDescending(e)).AsOrderedCacheOptimizedQueriable();
        }
               
        public static ICacheOptimizedQueryable<TResult> Select<TSource, TResult>(this ICacheOptimizedQueryable<TSource> source, Expression<Func<TSource, TResult>> selector)
        {
            var optimizedExpression = OptimizeExpressionForCache(selector);
            return source.AsQueryable().Select(optimizedExpression).AsCacheOptimizedQueriable();
        }

        public static ICacheOptimizedQueryable<TResult> SelectMany<TSource, TResult>(this ICacheOptimizedQueryable<TSource> source, Expression<Func<TSource, IEnumerable<TResult>>> selector) {
            return DecorateMethod(source, selector, (q, e) => q.SelectMany(e)).AsCacheOptimizedQueriable();
        }

        public static ICacheOptimizedQueryable<TResult> SelectMany<TSource, TCollection, TResult>(this ICacheOptimizedQueryable<TSource> source, Expression<Func<TSource, IEnumerable<TCollection>>> collectionSelector, Expression<Func<TSource, TCollection, TResult>> resultSelector) {
            var collectionSelectorOptimized = OptimizeExpressionForCache(collectionSelector);
            var resultSelectorOptimized = OptimizeExpressionForCache(resultSelector);
            return source.AsQueryable().SelectMany(collectionSelector, resultSelector).AsCacheOptimizedQueriable();
        }

        public static TSource Single<TSource>(this ICacheOptimizedQueryable<TSource> source, Expression<Func<TSource, bool>> predicate) {
            return DecorateMethod(source, predicate, (q, e) => q.Single(e));
        }

        public static TSource SingleOrDefault<TSource>(this ICacheOptimizedQueryable<TSource> source, Expression<Func<TSource, bool>> predicate) {
            return DecorateMethod(source, predicate, (q, e) => q.SingleOrDefault(e));
        }

        public static IOrderedCacheOptimizedQueryable<TSource> ThenBy<TSource, TKey>(this IOrderedCacheOptimizedQueryable<TSource> source, Expression<Func<TSource, TKey>> keySelector) {
            var keySelectorOptimized = OptimizeExpressionForCache(keySelector);
            return source.AsOrdetedQueriable().ThenBy(keySelectorOptimized).AsOrderedCacheOptimizedQueriable();
        }

        public static IOrderedCacheOptimizedQueryable<TSource> ThenByDescending<TSource, TKey>(this IOrderedCacheOptimizedQueryable<TSource> source, Expression<Func<TSource, TKey>> keySelector) {
            var keySelectorOptimized = OptimizeExpressionForCache(keySelector);
            return source.AsOrdetedQueriable().ThenByDescending(keySelectorOptimized).AsOrderedCacheOptimizedQueriable();
        }

        /// <inheritdoc cref="CacheOptimizedQueriableExtentions.Where{TSource}(IQueryable{TSource}, Expression{Func{TSource, bool}})"/>
        public static ICacheOptimizedQueryable<TSource> Where<TSource>(this ICacheOptimizedQueryable<TSource> source, Expression<Func<TSource, bool>> predicate)
        {
            return DecorateMethod(source, predicate, (q, e) => q.Where(e)).AsCacheOptimizedQueriable();
        }

        public static ICacheOptimizedQueryable<TSource> Where<TSource>(this ICacheOptimizedQueryable<TSource> source, Expression<Func<TSource, int, bool>> predicate)
        {
            return DecorateMethod(source, predicate, (q, e) => q.Where(e)).AsCacheOptimizedQueriable();
        }

        private static T OptimizeExpressionForCache<T>(T expression) where T : Expression
        {
            var visitor = new CacheOptimizedExpressionVisitor();
            var optimizedExpression = (T)visitor.Visit(expression);
            return optimizedExpression;
        }

        private static TOut DecorateMethod<TSource, TExpression, TOut>(this ICacheOptimizedQueryable<TSource> source, TExpression expression, Func<IQueryable<TSource>, TExpression, TOut> method) where TExpression : Expression
        {
            var optimizedExpression = OptimizeExpressionForCache(expression);
            var query = method(source.AsQueryable(), optimizedExpression);
            return query;
        }

    }
}
