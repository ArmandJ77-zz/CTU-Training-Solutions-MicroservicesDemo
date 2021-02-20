﻿using Cart.DTO;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System;
using System.Text;

namespace Cart.Events.Publishers
{
    public interface IItemAddedEventPublisher
    {
        void Publish(CartItemDto dto);
    }

    public class ItemAddedEventPublisher : IItemAddedEventPublisher
    {
        private const string QueueName = "cart-item-added"; 

        public void Publish(CartItemDto  dto)
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

            var body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(dto));

            channel.BasicPublish("", QueueName, null, body);
        }
    }
}
