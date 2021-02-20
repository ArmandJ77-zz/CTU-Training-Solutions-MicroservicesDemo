using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Products.DTO;

namespace Products.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProductsController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            return Ok(new List<ProductDto>
            {
                new ProductDto
                {
                    Id = 0,
                    Name = "Alienware M15 R3",
                    Qty = 5
                },
                new ProductDto
                {
                    Id = 1,
                    Name = "Asus ROG Zephyrus G14",
                    Qty = 4
                },
                new ProductDto
                {
                    Id = 3,
                    Name = "Razer Blade Pro",
                    Qty = 2
                }
            });
        }
    }
}
