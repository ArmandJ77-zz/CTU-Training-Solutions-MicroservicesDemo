using Orders.DTO;
using Orders.Events.Publishers;
using OrderEntity = Orders.Database.Entities.Order;

namespace Orders.Domain.Order
{
    public static class Mapper
    {
        public static OrderPlacedPublisher.OrderPlacedEvent ToOrderPlacedEvent(this OrderEntity entity)
            => new OrderPlacedPublisher.OrderPlacedEvent
            {
                OrderId = entity.Id,
                Qty = entity.Qty,
                ProductId = entity.ProductId
            };

        public static OrderDto ToDto(this OrderEntity entity)
            => new OrderDto
            {
                Id = entity.Id,
                Qty = entity.Qty,
                ProductId = entity.ProductId
            };

        public static OrderEntity ToEntity(this OrderDto dto)
            => new OrderEntity
            {
                Id = dto.Id,
                Qty = dto.Qty,
                ProductId = dto.ProductId
            };
    }
}
