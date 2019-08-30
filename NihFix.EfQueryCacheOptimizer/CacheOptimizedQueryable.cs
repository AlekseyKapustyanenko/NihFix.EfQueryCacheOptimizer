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
        private readonly IQueryable<T> _originalQueryable;

        public CacheOptimizedQueryable(IQueryable<T> originalQueryable)
        {
            _originalQueryable = originalQueryable;
        }
        public Expression Expression => _originalQueryable.Expression;

        public Type ElementType => _originalQueryable.ElementType;

        public IQueryProvider Provider => _originalQueryable.Provider;

        public IQueryable<T> AsQueryable()
        {
            return _originalQueryable;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _originalQueryable.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)_originalQueryable).GetEnumerator();
        }
    }
}
