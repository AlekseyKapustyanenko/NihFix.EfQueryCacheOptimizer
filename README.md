# NihFix.EfQueryCacheOptimizer
Sometimes EntityFramework dosn't save your query in query cache(second level cache). For example it is query that contsains Any(), All(), Contains() method calls, constant expressions or unary bool comparation(...Where(x=>x.IsActual));
This library can fix this issue. Just add .AsCacheOptimizedQueriable() to your entity store before build query.

Installation:
```Powershell
PM> Install-Package NihFix.EfQueryCacheOptimizer
```

Example:
```C#
var interestingIds=new[]{1,2,3};
context.Orders.AsCacheOptimizedQueryable().Where(o=>o.IsActual && interestingIds.Contains(o.Id));
```
or

```C#
var interestingIds=new[]{1,2,3};
context.Set<order>().AsCacheOptimizedQueryable().Where(o=>o.IsActual && interestingIds.Contains(o.Id));
```
# Collection optimization
Now items count is increased up to **OptimalCollectionSize** with first volue in collection. And if you pass collection in query with different collection size(less than **OptimalCollectionSize**), query would not recompile. Defafault **OptimalCollectionSize** is 10, but you can set your own **OptimalCollectionSize**.

Set OptimalCollectionSize with parameter:
```C#
var interestingIds=new[]{1,2,3};
var defaultConfig=new OptimizationConfig(){OptimalCollectionSize = 5};
context.Orders.AsCacheOptimizedQueryable(defaultConfig).Where(o=>o.IsActual && interestingIds.Contains(o.Id));
```
Set OptimalCollectionSize with config:
```XML
<configuration>
 <configSections>    
    <section name="queryCacheOptimizer" type="NihFix.EfQueryCacheOptimizer.Configuration.OptimizationConfigConfigSection, NihFix.EfQueryCacheOptimizer" requirePermission="false"/>
  </configSections>
 <queryCacheOptimizer optimalCollectionSize="5"/>
</configuration>

```
