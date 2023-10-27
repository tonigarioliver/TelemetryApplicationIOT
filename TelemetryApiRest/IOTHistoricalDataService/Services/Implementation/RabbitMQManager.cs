using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IOTHistoricalDataService.Config;
using Microsoft.Extensions.Options;

namespace IOTHistoricalDataService.Services.Implementation
{
    public class RabbitMQManager
    {
        private IConnection _connection;
        private IModel _channel;

        public RabbitMQManager(IOptions<RabbitMQSettings>rabbitMQSettings)
        {
            var factory = new ConnectionFactory
            {
                HostName = rabbitMQSettings.Value.Hostname,
                Port = rabbitMQSettings.Value.Port,
                UserName = rabbitMQSettings.Value.Username,
                Password = rabbitMQSettings.Value.Password
            };
            Console.WriteLine(rabbitMQSettings.Value.Hostname);

            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
        }
        
        public void PublishMessage(string exchange, string routingKey, byte[] messageBody)
        {
            _channel.BasicPublish(exchange, routingKey, null, messageBody);
        }

        public void Subscribe(string queueName, Action<byte[]> messageHandler)
        {
            _channel.QueueDeclare(queue: queueName, durable: false, exclusive: false, autoDelete: false, arguments: null);

            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += (model, ea) =>
            {
                byte[] body = ea.Body.ToArray(); // Convierte ReadOnlyMemory<byte> a byte[]
                messageHandler(body);
            };

            _channel.BasicConsume(queue: queueName, autoAck: true, consumer: consumer);
        }

        public void Dispose()
        {
            _channel.Dispose();
            _connection.Dispose();
        }
    }
}
