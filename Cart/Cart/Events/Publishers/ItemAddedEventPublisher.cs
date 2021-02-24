using Cart.DTO;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System;
using System.Text;

// TODO: Maybe look at a basic CRUD user service
// TODO: Link a user to a cart when placing an order need reserve an item for purchase (assume you can only qty limit = 1)
// TODO: Create Event to gather cart data and publish cart-checkout-event for orders to consume
// TODO: On orders save the order with a status of pending and publishes order-created-event
// TODO: Products listens on order-created-event checks stock, if qty > than in stock then publish that full order as product-out-stock
// TODO: Products listens on order-created-event checks stock, if qty < than in stock decrease amount and publish product-stock-confirm
// TODO: orders listens on product-out-stock and send email with out-of-stock notice
// TODO: Orders listens on product-stock-confirm and sends stock confirmation email

namespace Cart.Events.Publishers
{
    public interface IItemAddedEventPublisher
    {
        void Publish(CartItemDto dto);
    }

    public class ItemAddedEventPublisher : IItemAddedEventPublisher
    {
        private const string QueueName = "cart-item-added";

        public void Publish(CartItemDto dto)
        {
            var factory = new ConnectionFactory
            {
                Uri = new Uri("amqp://guest:guest@localhost:5672")
            };
            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();

            channel.QueueDeclare(QueueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            var cartItemAddedEvent = JsonConvert.SerializeObject(
                new CartItemAddedEvent
                {
                    ProductId = dto.ProductId,
                    Qty = dto.Qty
                });

            var body = Encoding.UTF8.GetBytes(cartItemAddedEvent);

            channel.BasicPublish("", QueueName, null, body);
        }
    }

    public class CartItemAddedEvent
    {
        public long ProductId { get; set; }
        public int Qty { get; set; }
    }
}
