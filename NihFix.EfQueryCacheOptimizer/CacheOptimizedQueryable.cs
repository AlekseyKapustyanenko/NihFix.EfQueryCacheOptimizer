using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace NihFix.EfQueryCacheOptimizer
{
    internal class CacheOptimizedQueryable<T> : IQueryable<T>, IOrderedQueryable<T>
    {
        private readonly IQueryable<T> _origilQueriable;

        public CacheOptimizedQueryable(IQueryable<T> origilQueriable)
        {
            _origilQueriable = origilQueriable;
            Expression = origilQueriable.Expression;
        }

        public CacheOptimizedQueryable(IQueryable<T> origilQueriable, Expression originalExpression)
        {
            _origilQueriable = origilQueriable;
            Expression = originalExpression;
        }
        public Expression Expression { get; }


        public Type ElementType => _origilQueriable.ElementType;

        public IQueryProvider Provider => new CacheOptimizedQueryProvider(_origilQueriable.Provider);

        public IQueryable<T> AsQueryable()
        {
            return _origilQueriable;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _origilQueriable.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)_origilQueriable).GetEnumerator();
        }

        
    }

    internal class CacheOptimizedQueryable : IQueryable
    {
        public readonly IQueryable _origilQueriable;

        public CacheOptimizedQueryable(IQueryable origilQueriable)
        {
            _origilQueriable = origilQueriable;
            Expression = origilQueriable.Expression;
        }

        public CacheOptimizedQueryable(IQueryable origilQueriable, Expression originalExpression)
        {
            _origilQueriable = origilQueriable;
            Expression = originalExpression;
        }

        public Expression Expression { get; }

        public Type ElementType => _origilQueriable.ElementType;

        public IQueryProvider Provider => new CacheOptimizedQueryProvider(_origilQueriable.Provider);

        public IQueryable AsQueryable()
        {
            return _origilQueriable;
        }

        public IEnumerator GetEnumerator()
        {
            return _origilQueriable.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)_origilQueriable).GetEnumerator();
        }
    }
}
