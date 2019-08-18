using NihFix.EfQueryCacheOptimizer.Extentions;
using NihFix.EfQueryCacheOptimizer.Sample.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace NihFix.EfQueryCacheOptimizer.Sample
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var context=new SampleContext())
            {
                context.Database.Log = Console.WriteLine;
                var xr = new[] { "a", "b" };
                Expression<Func<TestEntity, bool>> t = x => xr.Any(xri => xri == x.Name);
                
                var y= ((LambdaExpression)((MethodCallExpression)t.Body).Arguments[1]).Body.GetType();
                var query = context.TestEntities.AsCacheOptimizedQueriable().Where(t);
                query.ToList();
                Console.ReadLine();
            }
        }
    }
}
