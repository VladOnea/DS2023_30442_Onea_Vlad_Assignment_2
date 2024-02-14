
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MonitoringAndCommunication.Dto;
using MonitoringAndCommunication.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;

namespace MonitoringAndCommunication.Services
{
    public class RabbitMQConsumerService : IHostedService
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly IHubContext<SignalRHub> _hubContext;
        private readonly SignalRHub _signalRHub;

        private IConnection _connection;
        private IModel _channel;

        public RabbitMQConsumerService(IServiceScopeFactory serviceScopeFactory, IHubContext<SignalRHub> hubContext, SignalRHub signalRHub)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _hubContext = hubContext;
            _signalRHub = signalRHub;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            ConnectionFactory factory = new()
            {
                Uri = new Uri("amqp://guest:guest@localhost:5672"),
                ClientProvidedName = "Consumer App"
            };
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            string exchangeName = "Exchange";
            string routingKey = "routing-key";
            string queName = "Queue";

            _channel.ExchangeDeclare(exchangeName, ExchangeType.Direct);
            _channel.QueueDeclare(queName, false, false, false, null);
            _channel.QueueBind(queName, exchangeName, routingKey, null);

            var consumer = new EventingBasicConsumer(_channel);

            var counter = 0;
            var measurement_value = 0.0f;

            consumer.Received += async (model, ea) =>
            {
                using var scope = _serviceScopeFactory.CreateScope();
                var consumptionService = scope.ServiceProvider.GetRequiredService<IConsumptionService>();
                var deviceConsumptionService= scope.ServiceProvider.GetRequiredService<IDeviceConsumptionService>();
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                Console.WriteLine("Message: " + message);
                var data = JsonConvert.DeserializeObject<DeviceConsumptionDto>(message);

                Console.WriteLine($" Received {data}");
                counter++;
                if (counter % 6 == 0)
                {
                    EnergyConsumption energyConsumption = new EnergyConsumption() { Timestamp = data.Timestamp, DeviceId = data.DeviceId, Consumption = measurement_value };
                    await consumptionService.AddConsumption(energyConsumption);
                    var device= deviceConsumptionService.GetDevice(data.DeviceId);
                    if (measurement_value > float.Parse(device.MaxConsumption))
                    {
                        //Console.WriteLine("MaiMare: ");
                        if (device.UserId != null)
                        { 
                            int userId = (int)device.UserId;
                            await _signalRHub.SendMessageToUser("Device consumption over limit!", userId);
                        }
                    }
                }
                measurement_value += data.MeasurementValue;
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
