using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Products.Domain.Handler;
using Products.DTO;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Products.Database;

namespace Products.Events.Subscribers
{
    public class OrderPlacedSubscriber : BackgroundService
    {
        private ConnectionFactory _connectionFactory;
        private IConnection _connection;
        private IModel _channel;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IProductCheckHandler _handler;

        private const string QueueName = "order-placed";

        public OrderPlacedSubscriber(
            IServiceScopeFactory scopeFactory,
            IProductCheckHandler productCheckHandler)
        {
            _scopeFactory = scopeFactory;
            _handler = productCheckHandler;
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            _connectionFactory = new ConnectionFactory
            {
                UserName = "guest",
                Password = "guest"
            };
            _connection = _connectionFactory.CreateConnection();
            _channel = _connection.CreateModel();
            _channel.QueueDeclarePassive(QueueName);
            _channel.BasicQos(0, 1, false);

            return base.StartAsync(cancellationToken);
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();

            var timer = new Timer(ConsumeEvent, null, TimeSpan.Zero, TimeSpan.FromSeconds(5));

            return Task.CompletedTask;
        }

        private void ConsumeEvent(object state)
        {
            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += (sender, e) =>
            {
                var body = e.Body.ToArray();
                var orderPlacedEvent = JsonConvert.DeserializeObject<OrderPlacedEvent>(Encoding.UTF8.GetString(body));
                var dto = new ProductDto
                {
                    Id = orderPlacedEvent.ProductId,
                    Qty = orderPlacedEvent.Qty,
                    OrderId = orderPlacedEvent.ProductId
                };
                using var scope = _scopeFactory.CreateScope();
                var context = scope.ServiceProvider.GetService<ProductsDbContext>();

                _handler.Handle(context, dto);
            };

            _channel.BasicConsume(QueueName, true, consumer);
        }
    }

    public class OrderPlacedEvent
    {
        public long ProductId { get; set; }
        public int Qty { get; set; }
    }
}
