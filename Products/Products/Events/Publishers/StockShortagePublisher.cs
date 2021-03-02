using Newtonsoft.Json;
using RabbitMQ.Client;
using System;
using System.Text;

namespace Products.Events.Publishers
{
    public interface IStockShortagePublisher
    {
        void Publish(StockShortageEvent shortageEvent);
    }

    public class StockShortagePublisher : IStockShortagePublisher
    {
        private const string QueueName = "stock-shortage";

        public void Publish(StockShortageEvent shortageEvent)
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

            var body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(shortageEvent));

            channel.BasicPublish("", QueueName, null, body);
        }
    }

    public class StockShortageEvent
    {
        public long OrderId { get; set; }
        public long ProductId { get; set; }
    }
}
