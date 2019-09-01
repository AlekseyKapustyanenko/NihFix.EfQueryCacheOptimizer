using System.Configuration;

namespace NihFix.EfQueryCacheOptimizer.Configuration
{
    public class OptimizationConfigConfigSection : ConfigurationSection, IOptimizationConfig
    {
        [ConfigurationProperty( "optimalCollectionSize" )]
        public int OptimalCollectionSize => (int) base["optimalCollectionSize"];
    }
}