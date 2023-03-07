using Mango.MessageBus;

namespace Mango.Services.ShoppingCartAPI.RabbitMQSender;

public class RabbitMQCartMessageSender : IRabbitMQCartMessageSender
{
    public void SendMessage(BaseMessage message, string queueName)
    {
        throw new NotImplementedException();
    }
}
