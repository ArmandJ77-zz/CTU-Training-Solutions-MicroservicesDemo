using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Products.Database;
using Products.DTO;
using System;
using System.Threading.Tasks;

namespace Products.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly ProductsDbContext _dbContext;

        public ProductsController(ProductsDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        public async Task<IActionResult> GetAsync()
        {
            var result = await _dbContext.Products.ToListAsync();

            return Ok(result);
        }

        [HttpDelete("{id}/{qty}")]
        public async Task<IActionResult> DecreaseProductQuantityAsync(long id, int qty)
        {


            return Ok();

        }
    }
}
