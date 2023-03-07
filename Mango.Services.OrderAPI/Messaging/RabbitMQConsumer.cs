using Mango.Services.OrderAPI.Repositories;
using RabbitMQ.Client;
using System.Threading.Channels;

namespace Mango.Services.OrderAPI.Messaging;

public class RabbitMQConsumer : BackgroundService
{
    private readonly OrderRepository _orderRepository;
    private IConnection _connection;
    private IModel _channel;

    public RabbitMQConsumer(OrderRepository orderRepository)
    {
        _orderRepository = orderRepository;

        var connectionFactory = new ConnectionFactory
        {
            HostName = "localhost",
            UserName = "guest",
            Password = "guest",
        };

        _connection = connectionFactory.CreateConnection();
        _channel = _connection.CreateModel();
        _channel.QueueDeclare(queue: "checkoutqueue", false, false, false, null);
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        throw new NotImplementedException();
    }
}
