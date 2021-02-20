using Cart.DTO;
using Cart.Events.Publishers;
using Microsoft.AspNetCore.Mvc;

namespace Cart.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class CartController : ControllerBase
    {
        private readonly IItemAddedEventPublisher _itemAddedEvent;

        public CartController(IItemAddedEventPublisher itemAddedEvent)
        {
            _itemAddedEvent = itemAddedEvent;
        }

        [HttpGet]
        public IActionResult Get()
        {
            return Ok("Cart API v1");
        }

        [HttpPost]
        public IActionResult AddToCart([FromBody] CartItemDto dto)
        {
            _itemAddedEvent.Publish(dto);

            return CreatedAtAction(nameof(AddToCart), new { id = dto.Id }, dto);
        }
    }
}
