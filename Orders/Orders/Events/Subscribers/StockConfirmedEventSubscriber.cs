using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Orders.Domain.Stock.Handlers;
using Orders.DTO;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Orders.Events.Subscribers
{
    public class StockConfirmedEventSubscriber : BackgroundService
    {
        private readonly IStockConfirmedHandler _handler;
        private const string QueueName = "stock-confimed";

        public StockConfirmedEventSubscriber(IStockConfirmedHandler handler)
        {
            _handler = handler;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
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

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += async (sender, e) =>
            {
                var body = e.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                var cartItemAddedEvent =
                    JsonConvert.DeserializeObject<StockConfirmedEvent>(Encoding.UTF8.GetString(body));

                var dto = new StockConfirmedDto
                {
                    ProductId = cartItemAddedEvent.ProductId,
                    OrderId = cartItemAddedEvent.OrderId
                };

                _handler.Handle(dto);
            };

            channel.BasicConsume(QueueName, true, consumer);

            return Task.CompletedTask;
        }

        public class StockConfirmedEvent
        {
            public long ProductId { get; set; }
            public long OrderId { get; set; }
        }
    }
}