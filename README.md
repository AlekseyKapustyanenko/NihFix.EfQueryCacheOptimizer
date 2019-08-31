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
