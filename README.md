## About the Repo

- This project is follow-along + Chapter notes of this course:  
  https://www.linkedin.com/learning/building-web-apis-with-asp-dot-net-core-3

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

## 4. Writing Data

### REST with HTTP Verbs

| HTTP Verb |       Function       |
| --------- | :------------------: |
| GET       |      Read data       |
| POST      |   Create new data    |
| PUT       | Update existing data |
| DELETE    | Delete Existing data |

### Model Binding

1. [FromBody]: Data from http request (mostly POST/PUT)
1. [FromRoute]: Data from route template
1. [FromQuery]: Data from URL

### Model Validation

By Default [ApiController] handles automated HTTP validation rules. However if you want to disable this.
Update `Startup.cs` file like this:

```cs
 services.AddControllers()
            .ConfigureApiBehaviorOptions(options =>
            {
                options.SuppressModelStateInvalidFilter = true;
            });
        }
```

- You can use annotations.

```cs
[Required]
public string Name {get; set; }
[MaxLength(255)]
public string Description {get; set; }
```

### Adding Items

- Signature will be like this:

```cs
public async Task<ActionResult<Product>> PostProduct([FromBody] Product product)
        {
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
            return CreatedAtAction(
                "GetProduct",
                new { id = product.Id },
                product
            );
        }
```

### Updating Item

```cs
[HttpPut("{id}")]
public async Task<IActionResult> PutProduct(int id, [FromBody] Product product) {
    _context.Entry(product).State = EntityState.Modified;
    await _context.SaveChangesAsync();
    return NoContent();
}
```

### Deleting Item

- Usually returns deleted item

```cs
[HttpDelete("{id}")]
public async Task<IActionResult> DeleteProduct(int id) {
    _context.Products.Remove(p => p.Id==id);
    await _context.SaveChangesAsync();
}
```

## References

- https://www.linkedin.com/learning/building-web-apis-with-asp-dot-net-core-3

```

```
