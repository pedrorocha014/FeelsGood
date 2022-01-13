using System;
using System.Collections.Concurrent;
using System.Text;
using System.Text.Json;
using AnimalSelector.Data;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace AnimalSelector.AsyncDataService{
    public class MessageBusClient : IMessageBusClient
    {
        private readonly IConfiguration _configuration;
        private readonly IConnection _connection;
        private readonly IModel _channel;
        private readonly EventingBasicConsumer consumer;
        private readonly BlockingCollection<string> respQueue = new BlockingCollection<string>();
        private readonly IBasicProperties props;
        private readonly string replyQueueName;

        public MessageBusClient(IConfiguration configuration){
            _configuration = configuration;
            var factory = new ConnectionFactory(){
                HostName = "localhost"
            };

            try
            {
                _connection = factory.CreateConnection();
                _channel = _connection.CreateModel();

                replyQueueName = _channel.QueueDeclare().QueueName;
                consumer = new EventingBasicConsumer(_channel);
                
                props = _channel.CreateBasicProperties();
                var correlationId = Guid.NewGuid().ToString();
                props.CorrelationId = correlationId;
                props.ReplyTo = replyQueueName;

                _channel.ExchangeDeclare(exchange: "animal", type: ExchangeType.Topic);

                _channel.BasicConsume(
                    consumer: consumer,
                    queue: replyQueueName,
                    autoAck: true
                );

                consumer.Received += (model, ea) =>
                {
                    var body = ea.Body.ToArray();
                    var response = Encoding.UTF8.GetString(body);
                    if (ea.BasicProperties.CorrelationId == correlationId)
                    {
                        respQueue.Add(response);
                    }
                };

                _connection.ConnectionShutdown += RabbitMQ_ConnectionShutdown;

                Console.WriteLine($"--> Connected to MessageBus.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"--> Could not connect to the Message Bus: {ex.Message}.");
            }
        }

        public string PublishAnimalsRequest(ImageRequestDto imageRequest)
        {
            var message = JsonSerializer.Serialize(imageRequest);

            if(_connection.IsOpen){
                Console.WriteLine($"--> RabbitMQ Connection Open, sending message...");
                return SendMessage(message);
            }else{
                Console.WriteLine($"--> RabbitMQ Connection Closed, not sending.");
                return null;
            }
        }

        private string SendMessage(string message){
            var body = Encoding.UTF8.GetBytes(message);

            _channel.BasicPublish(exchange: "animal", 
                                routingKey: "external.animals.*",
                                basicProperties: props,
                                body: body);
            
            Console.WriteLine($"--> We have sent {message}");

            return respQueue.Take();
        }

        private void Dispose(){
            Console.WriteLine("MessageBus Disposed");

            if(_channel.IsOpen){
                _channel.Close();
                _connection.Close();
            }
        }

        private void RabbitMQ_ConnectionShutdown(object sender, ShutdownEventArgs e){
            Console.WriteLine($"--> RabbitMQ Connection Shutdown.");
        }
    }
}