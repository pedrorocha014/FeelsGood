using System;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;

namespace AnimalSelector.AsyncDataService{
    public class MessageBusClient : IMessageBusClient
    {
        private readonly IConfiguration _configuration;
        private readonly IConnection _connection;
        private readonly IModel _channel;

        public MessageBusClient(IConfiguration configuration){
            _configuration = configuration;
            var factory = new ConnectionFactory(){
                HostName = "localhost"
            };

            try
            {
                _connection = factory.CreateConnection();
                _channel = _connection.CreateModel();

                _channel.ExchangeDeclare(exchange: "trigger", type: ExchangeType.Topic);

                _connection.ConnectionShutdown += RabbitMQ_ConnectionShutdown;

                Console.WriteLine($"--> Connected to MessageBus.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"--> Could not connect to the Message Bus: {ex.Message}.");
            }
        }

        public void PublishAnimalsRequest()
        {
            var message = JsonSerializer.Serialize("");

            if(_connection.IsOpen){
                Console.WriteLine($"--> RabbitMQ Connection Open, sending message...");
                SendMessage(message);
            }else{
                Console.WriteLine($"--> RabbitMQ Connection Closed, not sending.");
            }
        }

        private void SendMessage(string message){
            var body = Encoding.UTF8.GetBytes(message);

            _channel.BasicPublish(exchange: "trigger", 
                                routingKey: "external.animals.*",
                                basicProperties: null,
                                body: body);
            
            Console.WriteLine($"--> We have sent {message}");
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