﻿using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

ConnectionFactory receiverFactory = new ConnectionFactory();
receiverFactory.Uri = new Uri("amqp:guest:guest@localhost:5672");
receiverFactory.ClientProvidedName = "Rabbit Receiver App 1";

IConnection receiverConnection = receiverFactory.CreateConnection();
IModel channel = receiverConnection.CreateModel();

const string exchangeName = "DemoExchange";
const string routingKey = "demo-routing-key";
const string queueName = "DemoQueue";

channel.ExchangeDeclare(exchangeName, ExchangeType.Direct);
channel.QueueDeclare(queueName, false, false, false, null);
channel.QueueBind(queueName, exchangeName, routingKey, null);
channel.BasicQos(0, 1, false);

var consumer = new EventingBasicConsumer(channel);
consumer.Received += (sender, args) =>
{
    Task.Delay(TimeSpan.FromSeconds(3)).Wait();
    var body = args.Body.ToArray();
    string message = Encoding.UTF8.GetString(body);
    Console.WriteLine($"Message received: {message}");
    channel.BasicAck(args.DeliveryTag, false);
};

string consumerTag = channel.BasicConsume(queueName, false, consumer);
Console.ReadLine();
channel.BasicCancel(consumerTag);

channel.Close();
receiverConnection.Close();