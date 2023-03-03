using Azure.Messaging.ServiceBus;
using Mango.MessageBus;
using Newtonsoft.Json;
using System.Text;

namespace Mango.Services.PaymentAPI.Messaging;

public class AzureServiceBusConsumer : IAzureServiceBusConsumer
{
    private readonly string _serviceBusConnectionString;
    private readonly string _CheckoutMessageTopic;
    private readonly string _subscriptionCheckout;
    private readonly string _orderpaymentprocesstopic;

    private readonly OrderRepository _orderRepository;
    private readonly IConfiguration _configuration;
    private readonly IMessageBus _messageBus;
    private ServiceBusProcessor _checkOutProcessor;

    public AzureServiceBusConsumer(OrderRepository orderRepository, IConfiguration configuration, IMessageBus messageBus)
    {
        _orderRepository = orderRepository;
        _configuration = configuration;
        _messageBus = messageBus;

        _serviceBusConnectionString = _configuration.GetValue<string>("ServiceBusConnectionString");
        _CheckoutMessageTopic = _configuration.GetValue<string>("CheckoutMessageTopic");
        _subscriptionCheckout = _configuration.GetValue<string>("SubscriptionCheckout");
        _orderpaymentprocesstopic = _configuration.GetValue<string>("OrderPaymentProcessTopic");

        var client = new ServiceBusClient(_serviceBusConnectionString);
        _checkOutProcessor = client.CreateProcessor(_CheckoutMessageTopic, _subscriptionCheckout);
    }

    public async Task Start()
    {
        _checkOutProcessor.ProcessMessageAsync += OnCheckOutMessageReceived;
        _checkOutProcessor.ProcessErrorAsync += ErrorHadler;
        await _checkOutProcessor.StartProcessingAsync();
    }

    public async Task Stop()
    {
        await _checkOutProcessor.StopProcessingAsync();
        await _checkOutProcessor.DisposeAsync();    
    }

    private Task ErrorHadler(ProcessErrorEventArgs arg)
    {
       Console.WriteLine(arg.Exception.ToString());
       return Task.CompletedTask;
    }

    private async Task OnCheckOutMessageReceived(ProcessMessageEventArgs args)
    {
        var message = args.Message;
        var body = Encoding.UTF8.GetString(message.Body);

        CheckoutHeaderDto checkoutHeaderDto = JsonConvert.DeserializeObject<CheckoutHeaderDto>(body)!;

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
            ExpiryMonthYear= orderHeader.ExpiryMonthYear,
            OrderId = orderHeader.OrderHeaderId,
            OrderTotal = orderHeader.OrderTotal,
        };

        try
        {
            await _messageBus.PublishMessage(paymentRequestMessage, _orderpaymentprocesstopic);
            await args.CompleteMessageAsync(args.Message);
        }
        catch (Exception ex)
        {

            Console.WriteLine(ex.ToString());   
        }
    }
}
