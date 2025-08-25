using System.Text;
using CommandsService.EventProcessing;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace CommandsService.AsyncDataServices;

// Builden a Background Service that will listen to the RabbitMQ message bus
// do not use Interface !

public class MessageBusSubscriber : BackgroundService, IAsyncDisposable
{
    private readonly IConfiguration _configuration;
    private readonly IEventProcessor _eventProcessor;
    private IConnection _connection;
    private IChannel _channel;
    private string _queueName;

    public MessageBusSubscriber(IConfiguration configuration, IEventProcessor eventProcessor)
    {
        _configuration = configuration;
        _eventProcessor = eventProcessor;
    }

    private async Task InitializeRabbitMQ()
    {
        var factory = new ConnectionFactory
        {
            HostName = _configuration["RabbitMQHost"],
            Port = int.Parse(_configuration["RabbitMQPort"]),
        };

        _connection = await factory.CreateConnectionAsync();
        _channel = await _connection.CreateChannelAsync();
        _channel.ExchangeDeclareAsync(exchange: "trigger", type: ExchangeType.Fanout);
        var queueDeclareOk = await _channel.QueueDeclareAsync(
            queue: "",
            durable: false,
            exclusive: true,
            autoDelete: true
        );
        _queueName = queueDeclareOk.QueueName; // RabbitMQ generiert einen Namen weil wir "" angegeben haben

        await _channel.QueueBindAsync(queue: _queueName, exchange: "trigger", routingKey: "");
        Console.WriteLine("---> Listening on the message bus...");

        _connection.ConnectionShutdownAsync += RabbitMQ_ConnectionShutdown;
    }

    private async Task RabbitMQ_ConnectionShutdown(object sender, ShutdownEventArgs e)
    {
        Console.WriteLine("---> RabbitMQ connection shutdown.");
        await Task.CompletedTask;
    }

    public override void Dispose()
    {
        if (_channel.IsOpen)
        {
            _channel.CloseAsync();
            _connection.CloseAsync();
        }
        base.Dispose();
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await InitializeRabbitMQ();
        stoppingToken.ThrowIfCancellationRequested();
        var consumer = new AsyncEventingBasicConsumer(_channel);
        consumer.ReceivedAsync += async (model, ea) =>
        {
            Console.WriteLine("---> Event received from RabbitMQ");
            var body = ea.Body;
            var notificationMessage = Encoding.UTF8.GetString(body.ToArray());
            _eventProcessor.ProcessEvent(notificationMessage);
            await Task.Yield(); // Ensure the event is processed asynchronously
        };

        await _channel.BasicConsumeAsync(queue: _queueName, autoAck: true, consumer: consumer);
        Console.WriteLine("---> Consumer started, waiting for messages...");
    }

    public async ValueTask DisposeAsync()
    {
        if (_channel.IsOpen)
        {
            await _channel.CloseAsync();
            await _connection.CloseAsync();
        }
    }
}
