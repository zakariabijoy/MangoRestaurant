using Azure.Messaging.ServiceBus;
using Mango.MessageBus;
using Mango.Services.PaymentAPI.Messages;
using Newtonsoft.Json;
using PaymentProcessor;
using System.Text;

namespace Mango.Services.PaymentAPI.Messaging;

public class AzureServiceBusConsumer : IAzureServiceBusConsumer
{
    private readonly string _serviceBusConnectionString;
    private readonly string _subscriptionPayment;
    private readonly string _orderpaymentprocesstopic;
    private readonly string _orderUpdatePaymentResultTopic;

    private readonly IProcessPayment _processPayment;
    private readonly IConfiguration _configuration;
    private readonly IMessageBus _messageBus;
    private readonly ServiceBusProcessor _orderPaymentProcessor;

    public AzureServiceBusConsumer(IProcessPayment processPayment, IConfiguration configuration, IMessageBus messageBus)
    {
        _processPayment = processPayment;
        _configuration = configuration;
        _messageBus = messageBus;

        _serviceBusConnectionString = _configuration.GetValue<string>("ServiceBusConnectionString");
        _subscriptionPayment = _configuration.GetValue<string>("OrderPaymentProcessSubscription");
        _orderpaymentprocesstopic = _configuration.GetValue<string>("OrderPaymentProcessTopic");
        _orderUpdatePaymentResultTopic = _configuration.GetValue<string>("OrderUpdatePaymentResultTopic");

        var client = new ServiceBusClient(_serviceBusConnectionString);
        _orderPaymentProcessor = client.CreateProcessor(_orderpaymentprocesstopic, _subscriptionPayment);
    }

    public async Task Start()
    {
        _orderPaymentProcessor.ProcessMessageAsync += ProcessPayments;
        _orderPaymentProcessor.ProcessErrorAsync += ErrorHadler;
        await _orderPaymentProcessor.StartProcessingAsync();
    }

    public async Task Stop()
    {
        await _orderPaymentProcessor.StopProcessingAsync();
        await _orderPaymentProcessor.DisposeAsync();    
    }

    private Task ErrorHadler(ProcessErrorEventArgs arg)
    {
       Console.WriteLine(arg.Exception.ToString());
       return Task.CompletedTask;
    }

    private async Task ProcessPayments(ProcessMessageEventArgs args)
    {
        var message = args.Message;
        var body = Encoding.UTF8.GetString(message.Body);

        PaymentRequestMessage paymentRequestMessage = JsonConvert.DeserializeObject<PaymentRequestMessage>(body)!;

        var result = _processPayment.PaymentProcessor();

        UpdatePaymentResultMessage updatePaymentResultMessage = new()
        {
            OrderId = paymentRequestMessage.OrderId,
            Status = result
        };


        try
        {
            await _messageBus.PublishMessage(updatePaymentResultMessage, _orderUpdatePaymentResultTopic);
            await args.CompleteMessageAsync(args.Message);
        }
        catch (Exception ex)
        {

            Console.WriteLine(ex.ToString());   
        }
    }
}
