using System.Text;
using System.Text.Json;
using PlatformServices.Dtos;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace PlatformService.AsyncDataServices;

public class MessageBusClient : IMessageBusClient
{
    IConfiguration _configuration;
    IConnection _connection;
    IChannel _channel;

    public MessageBusClient(IConfiguration configuration)
    {
        _configuration = configuration;

       
    }

    public async Task ConnectAsync()
    {
        var factory = new ConnectionFactory
        {
            HostName = _configuration["RabbitMQHost"],
            Port = int.Parse(_configuration["RabbitMQPort"]),
        };
        try
        {
            _connection = await factory.CreateConnectionAsync();
            _channel = await _connection.CreateChannelAsync();

            await _channel.ExchangeDeclareAsync(exchange: "trigger", type: ExchangeType.Fanout);
            _connection.ConnectionShutdownAsync += RabbitMQ_ConnectionShutdown;
            Console.WriteLine("----->Connected to the message bus.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"------>Could not connect to the message bus: {ex.Message}");
        }
    }

    private async Task RabbitMQ_ConnectionShutdown(object? sender, ShutdownEventArgs e)
    {
        Console.WriteLine("------>RabbitMQ connection shutdown.");
    }

    public async Task PublishNewPlatform(PlatformPublishedDto platformPublishedDto)
    {
        var message = JsonSerializer.Serialize(platformPublishedDto);

        if (_connection.IsOpen)
        {
            Console.WriteLine("--> RabitMQ connection open, sending message...");
            await SendMessage(message);
        }
        else
        {
            Console.WriteLine("--> RabbitMQ connection is closed, not sending message.");
            return;
        }
    }

    private async Task SendMessage(string message)
    {
        var body = Encoding.UTF8.GetBytes(message);
        await _channel.BasicPublishAsync(exchange: "trigger", routingKey: "", body: body);
    }

    public async Task Dispose()
    {
        if (_channel.IsOpen)
        {
            await _channel.CloseAsync();
            await _connection.CloseAsync();
        }
        Console.WriteLine("----->Message bus disposed.");
    }
}

public interface IMessageBusClient
{
    Task PublishNewPlatform(PlatformPublishedDto platformPublishedDto);
}
