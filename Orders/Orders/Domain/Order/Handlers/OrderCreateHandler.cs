using Microsoft.EntityFrameworkCore;
using Orders.DTO;
using Orders.Events.Publishers;

namespace Orders.Domain.Order.Handlers
{
    public interface IOrderCreateHandler
    {
        void Handle(DbContext dbContext, OrderDto dto);
    }

    public class OrderCreateHandler : IOrderCreateHandler
    {
        private readonly IOrderPlacedPublisher _orderPlacedPublisher;

        public OrderCreateHandler(IOrderPlacedPublisher orderPlacedPublisher)
        {
            _orderPlacedPublisher = orderPlacedPublisher;
        }

        public void Handle(DbContext dbContext, OrderDto dto)
        {
            var entity = dto.ToEntity();

            dbContext.Add(entity);
            dbContext.SaveChanges();

            var orderPlacedEvent = entity.ToOrderPlacedEvent();

            _orderPlacedPublisher.Publish(orderPlacedEvent);
        }
    }
}
