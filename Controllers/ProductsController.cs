using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using webapi_core.Classes;
using webapi_core.Models;

namespace webapi_core.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly ShopContext _context;
        public ProductsController(ShopContext context)
        {
            _context = context;
            _context.Database.EnsureCreated();
        }
        [HttpGet]
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

        // [HttpGet, Route("/products/{id}")]
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetProducts(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            return Ok(product);

        }

    }
}
