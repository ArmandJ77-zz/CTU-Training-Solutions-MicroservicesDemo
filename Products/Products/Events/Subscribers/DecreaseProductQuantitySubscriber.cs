using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Products.Database;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Products.Events.Subscribers
{
    public class DecreaseProductQuantitySubscriber : BackgroundService
    {
        private readonly ProductsDbContext _dbContext;
        private const string QueueName = "product-qty-decrease";

        public DecreaseProductQuantitySubscriber(ProductsDbContext dbContext)
        {
            _dbContext = dbContext;
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
                var decreaseProductQuantityEvent = JsonConvert.DeserializeObject<DecreaseProductQuantityEvent>(Encoding.UTF8.GetString(body));

                var product = await _dbContext.Products.FindAsync(decreaseProductQuantityEvent.Id);

                if (product == null)
                    return;

                if (decreaseProductQuantityEvent.Qty > product.Qty)
                    throw new ArgumentException("Decrease amount can not be greater than what's currently in stock");

                product.Qty = product.Qty - decreaseProductQuantityEvent.Qty;

                await _dbContext.SaveChangesAsync(stoppingToken);


            };

            channel.BasicConsume(QueueName, true, consumer);

            return Task.CompletedTask;
        }
    }

    public class DecreaseProductQuantityEvent
    {
        public long Id { get; set; }
        public int Qty { get; set; }
    }
}
