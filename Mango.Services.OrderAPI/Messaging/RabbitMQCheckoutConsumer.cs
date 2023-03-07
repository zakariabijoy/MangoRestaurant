﻿using Mango.MessageBus;
using Mango.Services.OrderAPI.Messages;
using Mango.Services.OrderAPI.Models;
using Mango.Services.OrderAPI.RabbitMQSender;
using Mango.Services.OrderAPI.Repositories;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Threading.Channels;

namespace Mango.Services.OrderAPI.Messaging;

public class RabbitMQCheckoutConsumer : BackgroundService
{
    private readonly OrderRepository _orderRepository;
    private readonly IRabbitMQOrderMessageSender _rabbitMQOrderMessageSender;
    private IConnection _connection;
    private IModel _channel;

    public RabbitMQCheckoutConsumer(OrderRepository orderRepository, IRabbitMQOrderMessageSender rabbitMQOrderMessageSender)
    {
        _orderRepository = orderRepository;
        _rabbitMQOrderMessageSender = rabbitMQOrderMessageSender;
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
        stoppingToken.ThrowIfCancellationRequested();

        var consumer = new EventingBasicConsumer(_channel);
        consumer.Received += (ch, ea) =>
        {
            var content = Encoding.UTF8.GetString(ea.Body.ToArray());
            CheckoutHeaderDto checkoutHeaderDto = JsonConvert.DeserializeObject<CheckoutHeaderDto>(content)!;
            HandleMessage(checkoutHeaderDto).GetAwaiter().GetResult();

            _channel.BasicAck(ea.DeliveryTag, false);
        };

        _channel.BasicConsume("checkoutqueue", false, consumer);

        return Task.CompletedTask;
    }

    private async Task HandleMessage(CheckoutHeaderDto checkoutHeaderDto)
    {
        OrderHeader orderHeader = new()
        {
            UserId = checkoutHeaderDto.UserId,
            FirstName = checkoutHeaderDto.FirstName,
            LastName = checkoutHeaderDto.LastName,
            OrderDetails = new List<OrderDetails>(),
            CardNumber = checkoutHeaderDto.CardNumber,
            CouponCode = checkoutHeaderDto.CouponCode,
            CVV = checkoutHeaderDto.CVV,
            DiscountTotal = checkoutHeaderDto.DiscountTotal,
            Email = checkoutHeaderDto.Email,
            ExpiryMonthYear = checkoutHeaderDto.ExpiryMonthYear,
            OrderTime = DateTime.Now,
            OrderTotal = checkoutHeaderDto.OrderTotal,
            PaymentStatus = false,
            Phone = checkoutHeaderDto.Phone,
            PickupDateTime = checkoutHeaderDto.PickupDateTime,
        };

        foreach (var detailList in checkoutHeaderDto.CartDetails)
        {
            OrderDetails orderDetails = new()
            {
                ProductId = detailList.ProductId,
                ProductName = detailList.Product.Name,
                Price = detailList.Product.Price,
                Count = detailList.Count
            };
            orderHeader.CartTotalItems += detailList.Count;
            orderHeader.OrderDetails.Add(orderDetails);
        }

        await _orderRepository.AddOrder(orderHeader);

        PaymentRequestMessage paymentRequestMessage = new()
        {
            Name = orderHeader.FirstName + " " + orderHeader.LastName,
            CardNumber = orderHeader.CardNumber,
            CVV = orderHeader.CVV,
            ExpiryMonthYear = orderHeader.ExpiryMonthYear,
            OrderId = orderHeader.OrderHeaderId,
            OrderTotal = orderHeader.OrderTotal,
            Email = orderHeader.Email,
        };

        try
        {
            _rabbitMQOrderMessageSender.SendMessage(paymentRequestMessage, "orderpaymentprocesstopic");
        }
        catch (Exception ex)
        {

            Console.WriteLine(ex.ToString());
        }
    }
}
