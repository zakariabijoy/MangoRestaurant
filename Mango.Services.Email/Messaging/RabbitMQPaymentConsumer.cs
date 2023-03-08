﻿using Mango.Services.Email.Messages;
using Mango.Services.Email.Repositories;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace Mango.Services.Email.Messaging;

public class RabbitMQPaymentConsumer : BackgroundService
{
    private const string ExchangeName = "PublishSubscribePaymentUpdate_Exchange";
    private readonly EmailRepository _emailRepository;
    private IConnection _connection;
    private IModel _channel;
    string queueName = "";

    public RabbitMQPaymentConsumer(EmailRepository emailRepository)
    {
        _emailRepository = emailRepository;
        var connectionFactory = new ConnectionFactory
        {
            HostName = "localhost",
            UserName = "guest",
            Password = "guest",
        };

        _connection = connectionFactory.CreateConnection();
        _channel = _connection.CreateModel();
        _channel.ExchangeDeclare(ExchangeName,ExchangeType.Fanout);
        queueName = _channel.QueueDeclare().QueueName;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        stoppingToken.ThrowIfCancellationRequested();

        var consumer = new EventingBasicConsumer(_channel);
        consumer.Received += (ch, ea) =>
        {
            var content = Encoding.UTF8.GetString(ea.Body.ToArray());
            UpdatePaymentResultMessage updatePaymentResultMessage = JsonConvert.DeserializeObject<UpdatePaymentResultMessage>(content)!;
            HandleMessage(updatePaymentResultMessage).GetAwaiter().GetResult();

            _channel.BasicAck(ea.DeliveryTag, false);
        };

        _channel.BasicConsume(queueName, false, consumer);

        return Task.CompletedTask;
    }

    private async Task HandleMessage(UpdatePaymentResultMessage updatePaymentResultMessage)
    {
        try
        {
            await _emailRepository.SendAndLogEmail(updatePaymentResultMessage);
        }
        catch (Exception ex)
        {

            Console.WriteLine(ex.ToString());
        }
    }
}
