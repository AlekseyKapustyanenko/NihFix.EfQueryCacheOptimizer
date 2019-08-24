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
                Expression<Func<TestEntity, xxx>> exp1 = t => new xxx { IsActual = t.IsActual };
                Expression<Func<TestEntity, bool>> exp = t =>  !t.IsActual && t.Id == 1;
                var query = context.TestEntities.Select(t=>t.Id).Average()
                //query.ToList();
                Console.ReadLine();
            }
        }

        public class xxx
        {
            public int Id { get; set; }

            public string Name { get; set; }

            public bool IsActual { get; set; }
        }
    }
}
