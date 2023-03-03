using Mango.MessageBus;

namespace Mango.Services.PaymentAPI.Messages;


public class PaymentRequestMessage : BaseMessage
{
    public PaymentRequestMessage()
    {
        Id = new Random().Next(100000, 199999);
        MessageCreated = DateTime.Now;
    }

    public int OrderId { get; set; }
    public string Name { get; set; }
    public string CardNumber { get; set; }
    public string CVV { get; set; }
    public string ExpiryMonthYear { get; set; }
    public double OrderTotal { get; set; }
    
}
