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
