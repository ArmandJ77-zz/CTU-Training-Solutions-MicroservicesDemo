using Newtonsoft.Json;
using RabbitMQ.Client;
using System;
using System.Text;

namespace Orders.Events.Publishers
{
    public interface IOrderPlacedPublisher
    {
        void Publish(OrderPlacedPublisher.OrderPlacedEvent orderEvent);
    }

    public class OrderPlacedPublisher : IOrderPlacedPublisher
    {
        private const string QueueName = "order-placed";

        public void Publish(OrderPlacedEvent orderEvent)
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

            var cartItemAddedEvent = JsonConvert.SerializeObject(orderEvent);

            var body = Encoding.UTF8.GetBytes(cartItemAddedEvent);

            channel.BasicPublish("", QueueName, null, body);
        }

        public class OrderPlacedEvent
        {
            public long OrderId { get; set; }
            public long ProductId { get; set; }
            public int Qty { get; set; }
        }
    }
}
