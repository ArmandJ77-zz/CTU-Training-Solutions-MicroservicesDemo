using Products.Database;
using Products.DTO;
using Products.Events.Publishers;

namespace Products.Domain.Handler
{
    public interface IProductCheckHandler
    {
        void Handle(ProductsDbContext context, ProductDto dto);
    }

    public class ProductCheckHandler : IProductCheckHandler
    {
        private readonly IStockShortagePublisher _stockShortagePublisher;
        private readonly IStockConfirmedPublisher _stockConfirmedPublisher;

        public ProductCheckHandler(
            IStockShortagePublisher stockShortagePublisher,
            IStockConfirmedPublisher stockConfirmedPublisher)
        {
            _stockShortagePublisher = stockShortagePublisher;
            _stockConfirmedPublisher = stockConfirmedPublisher;
        }

        public void Handle(ProductsDbContext context, ProductDto dto)
        {
            var product = context.Products.Find(dto.Id);

            if (product == null)
                return;

            if (dto.Qty > product.Qty)
            {
                _stockShortagePublisher.Publish(new StockShortageEvent
                {
                    OrderId = dto.OrderId,
                    ProductId = product.Id
                });

                return;
            }

            product.Qty -= dto.Qty;

            context.SaveChanges();

            _stockConfirmedPublisher.Publish(new StockConfirmedEvent
            {
                OrderId = product.Id,
                ProductId = dto.OrderId
            });
        }
    }
}
