using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using NihFix.EfQueryCacheOptimizer.Configuration;

namespace NihFix.EfQueryCacheOptimizer
{
    internal class CacheOptimizedQueryable<T> : ICacheOptimizedQueryable<T>
    {
        private const int OptimalCollectionSize = 10;

        private static readonly IOptimizationConfig _defaultConfig;
        
        private readonly IQueryable<T> _originalQueryable;

        static CacheOptimizedQueryable()
        {
            var defaultConfig = (OptimizationConfigConfigSection)ConfigurationManager.GetSection("queryCacheOptimizer");
            if (defaultConfig.ElementInformation.IsPresent)
            {
                _defaultConfig = defaultConfig;
            }
            else
            {
                _defaultConfig=new OptimizationConfig(){OptimalCollectionSize = OptimalCollectionSize};
            }
        }
        
        public CacheOptimizedQueryable(IQueryable<T> originalQueryable)
        {
            _originalQueryable = originalQueryable;
            OptimizationConfig = _defaultConfig;
        }
        
        public CacheOptimizedQueryable(IQueryable<T> originalQueryable, IOptimizationConfig optimizationConfig)
        {
            _originalQueryable = originalQueryable;
            OptimizationConfig = optimizationConfig;
        }

        public Expression Expression => _originalQueryable.Expression;

        public Type ElementType => _originalQueryable.ElementType;

        public IQueryProvider Provider => _originalQueryable.Provider;

        public IQueryable<T> AsQueryable()
        {
            return _originalQueryable;
        }

        public IOptimizationConfig OptimizationConfig { get; }

        public IEnumerator<T> GetEnumerator()
        {
            return _originalQueryable.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable) _originalQueryable).GetEnumerator();
        }
    }
}