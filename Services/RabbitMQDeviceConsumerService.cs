using MonitoringAndCommunication.Dto;
using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using System;
using System.Text;
using Newtonsoft.Json;
using MonitoringAndCommunication.Models;

namespace MonitoringAndCommunication.Services
{
    
    public class RabbitMQDeviceConsumerService : IHostedService
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;

        private IConnection _connection;
        private IModel _channel;

        public RabbitMQDeviceConsumerService(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            ConnectionFactory factory = new()
            {
                Uri = new Uri("amqp://guest:guest@host.docker.internal:5672"),
                ClientProvidedName = "Consumer App"
            };
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            string exchangeName = "ExchangeDevice";
            string routingKey = "device_routing_key";
            string queName = "DeviceQueue";

            _channel.ExchangeDeclare(exchangeName, ExchangeType.Direct);
            _channel.QueueDeclare(queName, false, false, false, null);
            _channel.QueueBind(queName, exchangeName, routingKey, null);

            var consumer = new EventingBasicConsumer(_channel);

            consumer.Received += async (model, ea) =>
            {
                using var scope = _serviceScopeFactory.CreateScope();
                var consumptionService = scope.ServiceProvider.GetRequiredService<IDeviceConsumptionService>();
                var existingDevices = consumptionService.GetAll();
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                Console.WriteLine("Message: " + message);
                var data = JsonConvert.DeserializeObject<SyncDeviceDto>(message);

                Console.WriteLine($" Received {data}");
                if (!existingDevices.Any(d => d.DeviceID == data.DeviceId))
                {
                    DeviceConsumption deviceConsumption = new DeviceConsumption() { DeviceID = data.DeviceId, MaxConsumption = data.MaxConsumption, UserId = data.UserId };
                    await consumptionService.AddConsumption(deviceConsumption);
                }


            };

            Console.WriteLine($" Received ");
            _channel.BasicConsume(queue: queName, autoAck: true, consumer: consumer);

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _channel?.Dispose();
            _connection?.Dispose();
            return Task.CompletedTask;
        }
    }
}

