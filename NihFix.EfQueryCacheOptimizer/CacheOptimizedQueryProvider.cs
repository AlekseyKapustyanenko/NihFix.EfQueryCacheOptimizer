using NihFix.EfQueryCacheOptimizer.Visitor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace NihFix.EfQueryCacheOptimizer
{
    public class CacheOptimizedQueryProvider : IQueryProvider
    {
        private readonly IQueryProvider _queryProvider;
        private readonly CacheOptimizedExpressionVisitor _visitor;

        public CacheOptimizedQueryProvider(IQueryProvider queryProvider)
        {
            _queryProvider = queryProvider;
            _visitor = new CacheOptimizedExpressionVisitor();
        }

        public IQueryable CreateQuery(Expression expression)
        {
            var optimizedExpression = _visitor.Visit(expression);
            return new CacheOptimizedQueryable(_queryProvider.CreateQuery(optimizedExpression), expression);
        }

        public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
        {
            var optimizedExpression = _visitor.Visit(expression);
            return new CacheOptimizedQueryable<TElement>(_queryProvider.CreateQuery<TElement>(optimizedExpression), expression);
        }

        public object Execute(Expression expression)
        {
            var optimizedExpression = _visitor.Visit(expression);
            return _queryProvider.Execute(optimizedExpression);
        }

        public TResult Execute<TResult>(Expression expression)
        {
            var optimizedExpression = _visitor.Visit(expression);
            return _queryProvider.Execute<TResult>(optimizedExpression);
        }
    }
}
