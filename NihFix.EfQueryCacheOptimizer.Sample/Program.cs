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
            using (var context = new SampleContext())
            {
                context.Database.Log = Console.WriteLine;
                var xr = new[] { 1, 2 };
                var query = context.TestEntities.AsCacheOptimizedQueriable().Select(x => new { Id = x.Id, Name = "qwert" });
                query.ToList();
                Console.ReadLine();
            }
        }

        public class xxx
        {
            public int Id { get; set; }

            public string Name { get; set; }
        }
    }
}
