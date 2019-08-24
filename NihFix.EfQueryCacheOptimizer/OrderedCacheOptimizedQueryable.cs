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
        private readonly IOrderedQueryable<T> _orderedQueriable;

        public OrderedCacheOptimizedQueryable(IOrderedQueryable<T> orderedQueriable):base(orderedQueriable)
        {
            orderedQueriable = _orderedQueriable;
        }
       
        public IOrderedQueryable<T> AsOrdetedQueriable()
        {
            return _orderedQueriable;
        }       

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)_orderedQueriable).GetEnumerator();
        }
    }
}
