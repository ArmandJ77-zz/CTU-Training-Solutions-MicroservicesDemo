using Newtonsoft.Json;
using RabbitMQ.Client;
using System;
using System.Text;

namespace Products.Events.Publishers
{
    public interface IStockConfirmedPublisher
    {
        void Publish(StockConfirmedEvent stockConfirmedEvent);
    }

    public class StockConfirmedPublisher: IStockConfirmedPublisher
    {
        private const string QueueName = "stock-confimed";

        public void Publish(StockConfirmedEvent stockConfirmedEvent)
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

            var body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(stockConfirmedEvent));

            channel.BasicPublish("", QueueName, null, body);
        }
    }
    public class StockConfirmedEvent
    {
        public long ProductId { get; set; }
        public long OrderId { get; set; }
    }
}
