using RabbitMQ.Client;
using System.Text;
using System;
using System.IO;


class Program
{
    static int myAtomicInteger = 0;
    static void Main()
    {

        ConnectionFactory factory = new();
        factory.Uri = new Uri("amqp://guest:guest@localhost:5672");
        factory.ClientProvidedName = "Sender App";

        using IConnection con = factory.CreateConnection();

        using (IModel model = con.CreateModel())
        {
            string exchangeName = "Exchange";
            string routingKey = "routing-key";
            string queName = "Queue";
            int deviceId = 21;

            model.ExchangeDeclare(exchangeName, ExchangeType.Direct);
            model.QueueDeclare(queName, false, false, false, null);
            model.QueueBind(queName, exchangeName, routingKey, null);

            using (var reader = new StreamReader("D:\\DSProject\\Sender\\sensor.csv"))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    DateTime currentTime = DateTime.Now.AddMinutes(myAtomicInteger * 9).AddMilliseconds(55000 * myAtomicInteger);
                    myAtomicInteger++;
                    string timestamp = currentTime.ToString("HH:mm:ss");
                    string measurementValue = line;
                    string jsonMessage = $"{{\"timestamp\":\"{timestamp}\",\"DeviceId\":{deviceId},\"measurementValue\":{measurementValue}}}";
                    byte[] messageBodyBytes = Encoding.UTF8.GetBytes(jsonMessage);
                    model.BasicPublish(exchangeName, routingKey, null, messageBodyBytes);
                    Console.WriteLine($" Sent '{jsonMessage}'");
                    Thread.Sleep(5000);
                }
            }
            model.Close();
            con.Close();
        }
    }
}