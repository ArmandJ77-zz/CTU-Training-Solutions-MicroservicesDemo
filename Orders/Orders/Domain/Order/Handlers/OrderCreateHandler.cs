using Microsoft.EntityFrameworkCore;
using Orders.DTO;
using Orders.Events.Publishers;
using System.Threading;
using System.Threading.Tasks;

namespace Orders.Domain.Order.Handlers
{
    public class OrderCreateHandler : IOrderCreateHandler
    {
        private readonly IOrderPlacedPublisher _orderPlacedPublisher;

        public OrderCreateHandler(IOrderPlacedPublisher orderPlacedPublisher)
        {
            _orderPlacedPublisher = orderPlacedPublisher;
        }

        public async Task HandleAsync(DbContext dbContext, OrderDto dto, CancellationToken cancellationToken)
        {
            var entity = dto.ToEntity();

            await dbContext.AddAsync(entity, cancellationToken);
            await dbContext.SaveChangesAsync(cancellationToken);

            var orderPlacedEvent = entity.ToOrderPlacedEvent();
            _orderPlacedPublisher.Publish(orderPlacedEvent);
        }
    }

    public interface IOrderCreateHandler
    {
        Task HandleAsync(DbContext dbContext, OrderDto dto, CancellationToken cancellationToken);
    }
}
