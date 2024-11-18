using RabbitMQ.Client;
using System.Text;

ConnectionFactory senderFactory = new ConnectionFactory();
senderFactory.Uri = new Uri("amqp://guest:guest@localhost:5672");
senderFactory.ClientProvidedName = "Rabbit Sender App";

IConnection senderConnection = senderFactory.CreateConnection();
IModel channel = senderConnection.CreateModel();

const string exchangeName = "DemoExchange";
const string routingKey = "demo-routing-key";
const string queueName = "DemoQueue";

channel.ExchangeDeclare(exchangeName, ExchangeType.Direct);
channel.QueueDeclare(queueName, false, false, false, null);
channel.QueueBind(queueName, exchangeName, routingKey, null);

for (int i = 0; i < 10; i++)
{
    Console.WriteLine($"Sending message {i}");
    byte[] messageBodyBytes = Encoding.UTF8.GetBytes($"Hello RabbitMQ #{i}");
    channel.BasicPublish(exchangeName, routingKey, null, messageBodyBytes);
    Thread.Sleep(1000);
}

channel.Close();
senderConnection.Close();