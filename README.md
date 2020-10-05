## 3. Advanced Data Retrieval

### Pagination

> `https://localhost:5000/products?size=15&page=`  
> `.Skip(queryParameters.Size * (queryParameters.Page - 1))`

- Using hints for query parameters (`[FromQuery]`)

```cs
    public async Task<IActionResult> GetAllProducts([FromQuery] QueryParameters queryParameters)
        {
            IQueryable<Product> products = _context.Products;
            products = products
            .Skip(queryParameters.Size * (queryParameters.Page - 1))
            .Take(queryParameters.Size); ;
            return Ok(await products.ToArrayAsync());

        }
```

### Filtering

- Parameter Class
- Eg: MinPrice, MaxPrice
  decimal?
  Apply Where

```cs
        public async Task<IActionResult> GetAllProducts([FromQuery] ProductQueryParameters queryParameters)
        {
            IQueryable<Product> products = _context.Products;

            if ((queryParameters.MinPrice != null) && (queryParameters.MaxPrice != null))
            {
                products = products.Where(
                    p => p.Price >= queryParameters.MinPrice.Value &&
                    p.Price <= queryParameters.MaxPrice.Value);

            }
            if (!string.IsNullOrEmpty(queryParameters.Sku))
            {
                products = products.Where(p => p.Sku == queryParameters.Sku);
            }

            products = products
            .Skip(queryParameters.Size * (queryParameters.Page - 1))
            .Take(queryParameters.Size);
            return Ok(await products.ToArrayAsync());

        }
```

### Searching

```cs
.Where(p => p.Name.ToLower().Contains(
    queryParameters.Name.ToLower()
));
```

### Sorting

- Field is dynamic, we will use extensions.
- Extension enables sorting by arbitrary property by dynamically building lambda expressions.
  > `https://localhost:5001/products?sortBy=Price&sortOrder=desc`
- Building Extension. [Models > IQueryableExtensions.cs]

```cs
    public static class IQueryableExtensions
    {
        public static IQueryable<TEntity> OrderByCustom<TEntity>(this IQueryable<TEntity> items, string sortBy, string sortOrder)
        {

            var type = typeof(TEntity);
            var expression2 = Expression.Parameter(type, "t");
            var property = type.GetProperty(sortBy);
            var expression1 = Expression.MakeMemberAccess(expression2, property);
            var lambda = Expression.Lambda(expression1, expression2);
            var result = Expression.Call(
                typeof(Queryable),
                sortOrder == "desc" ? "orderByDescending" : "OrderBy",
                new Type[] { type, property.PropertyType },
                items.Expression,
                Expression.Quote(lambda)
            );
            return items.Provider.CreateQuery<TEntity>(result);

        }
    }
```

- Add Sorting Layer

```cs
if (!string.IsNullOrEmpty(queryParameters.SortBy))
            {
                if (typeof(Product).GetProperty(queryParameters.SortBy) != null)
                {
                    products = products.OrderByCustom(queryParameters.SortBy, queryParameters.SortOrder);
                }
            }
```
