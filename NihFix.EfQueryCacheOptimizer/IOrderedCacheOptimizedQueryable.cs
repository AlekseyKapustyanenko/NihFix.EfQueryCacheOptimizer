using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Linq
{
    public interface IOrderedCacheOptimizedQueryable<out T> : ICacheOptimizedQueryable<T>
    {
        IOrderedQueryable<T> AsOrdetedQueriable();
    }
}
