using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace NihFix.EfQueryCacheOptimizer
{
    internal class OrderedCacheOptimizedQueryable<T> : CacheOptimizedQueryable<T>, IOrderedCacheOptimizedQueryable<T>
    {
        private readonly IOrderedQueryable<T> _orderedQueryable;

        public OrderedCacheOptimizedQueryable(IOrderedQueryable<T> orderedQueryable) : base(orderedQueryable)
        {
            _orderedQueryable = orderedQueryable;
        }
        
        public OrderedCacheOptimizedQueryable(
            IOrderedQueryable<T> orderedQueryable, 
            IOptimizationConfig optimizationConfig):base(orderedQueryable, optimizationConfig)
        {
            _orderedQueryable = orderedQueryable;
        }
        public IOrderedQueryable<T> AsOrderedQueryable()
        {
            return _orderedQueryable;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)_orderedQueryable).GetEnumerator();
        }
    }
}
