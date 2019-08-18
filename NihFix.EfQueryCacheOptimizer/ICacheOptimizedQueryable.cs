using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NihFix.EfQueryCacheOptimizer
{
    public interface ICacheOptimizedQueryable<out T>:IQueryable<T>
    {
        IQueryable<T> AsQueryable();
    }    
}
