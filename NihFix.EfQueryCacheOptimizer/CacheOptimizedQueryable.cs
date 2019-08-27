using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace NihFix.EfQueryCacheOptimizer
{
    internal class CacheOptimizedQueryable<T> : ICacheOptimizedQueryable<T>
    {
        public readonly IQueryable<T> _origilQueriable;

        public CacheOptimizedQueryable(IQueryable<T> origilQueriable)
        {
            _origilQueriable = origilQueriable;
        }
        public Expression Expression => _origilQueriable.Expression;

        public Type ElementType => _origilQueriable.ElementType;

        public IQueryProvider Provider => _origilQueriable.Provider;

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
}
