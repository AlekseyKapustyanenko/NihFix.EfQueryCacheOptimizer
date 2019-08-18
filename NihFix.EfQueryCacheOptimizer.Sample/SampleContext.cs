using NihFix.EfQueryCacheOptimizer.Sample.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NihFix.EfQueryCacheOptimizer.Sample
{
    public class SampleContext:DbContext
    {

        public DbSet<TestEntity> TestEntities { get; set; }
    }
}
