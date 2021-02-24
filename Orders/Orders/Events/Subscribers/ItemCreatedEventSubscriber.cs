﻿using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Orders.Database;
using Orders.Domain.Order.Handlers;
using Orders.DTO;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Orders.Events.Subscribers
{
    public class ItemCreatedEventSubscriber : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IOrderCreateHandler _orderCreateHandler;
        private const string QueueName = "cart-item-added";

        public ItemCreatedEventSubscriber(IServiceScopeFactory scopeFactory, IOrderCreateHandler orderCreateHandler)
        {
            _scopeFactory = scopeFactory;
            _orderCreateHandler = orderCreateHandler;
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
                var cartItemAddedEvent = JsonConvert.DeserializeObject<CartItemAddedEvent>(Encoding.UTF8.GetString(body));

                var order = new OrderDto
                {
                    ProductId = cartItemAddedEvent.ProductId,
                    Qty = cartItemAddedEvent.Qty
                };

                using var scope = _scopeFactory.CreateScope();
                var context = scope.ServiceProvider.GetService<OrdersDbContext>();
                await _orderCreateHandler.HandleAsync(context, order, stoppingToken);
            };

            channel.BasicConsume(QueueName, true, consumer);

            return Task.CompletedTask;
        }

        public class CartItemAddedEvent
        {
            public long ProductId { get; set; }
            public int Qty { get; set; }
        }
    }
}
