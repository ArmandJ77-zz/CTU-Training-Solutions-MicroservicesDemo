using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Orders.Database;
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
    public class StockShortageEventSubscription : BackgroundService
    {
        private ConnectionFactory _connectionFactory;
        private IConnection _connection;
        private IModel _channel;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IStockShortageHandler _handler;
        private const string QueueName = "stock-shortage";

        public StockShortageEventSubscription(IStockShortageHandler handler, IServiceScopeFactory scopeFactory)
        {
            _handler = handler;
            _scopeFactory = scopeFactory;
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
                var cartItemAddedEvent = JsonConvert.DeserializeObject<StockShortageEvent>(Encoding.UTF8.GetString(body));
                var dto = new StockShortageDto
                {
                    ProductId = cartItemAddedEvent.ProductId,
                    OrderId = cartItemAddedEvent.OrderId
                };

                using var scope = _scopeFactory.CreateScope();
                var context = scope.ServiceProvider.GetService<OrdersDbContext>();

                _handler.Handle(context, dto);
            };

            _channel.BasicConsume(QueueName, true, consumer);
        }

        public class StockShortageEvent
        {
            public long OrderId { get; set; }
            public long ProductId { get; set; }
        }
    }
}