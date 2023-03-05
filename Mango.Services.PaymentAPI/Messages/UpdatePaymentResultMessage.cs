using Mango.MessageBus;

namespace Mango.Services.PaymentAPI.Messages;

public class UpdatePaymentResultMessage : BaseMessage
{
    public UpdatePaymentResultMessage()
    {
        Id = new Random().Next(100000, 199999);
        MessageCreated = DateTime.Now;
    }

    public int OrderId { get; set; }
    public bool Status { get; set; }
    public string Email { get; set; }
}
