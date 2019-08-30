using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// ReSharper disable once CheckNamespace
namespace System.Linq
{
    public interface ICacheOptimizedQueryable<out T>:IQueryable<T>
    {
        IQueryable<T> AsQueryable();
    }    
}
