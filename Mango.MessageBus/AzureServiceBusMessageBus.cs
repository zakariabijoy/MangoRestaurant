using Azure.Messaging.ServiceBus;
using Newtonsoft.Json;
using System.Text;

namespace Mango.MessageBus;
public class AzureServiceBusMessageBus : IMessageBus
{
    // can be imroved by moving it to appsettings
    private string _connectionString = "Endpoint=sb://mangorestaurent.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=JlOGcdMiwKLkncX6EvBfzbDd1iq7OVu5Q+ASbL1ur7Y=";
    public async Task PublishMessage(BaseMessage message, string topicName)
    {
        await using var client = new ServiceBusClient(_connectionString);

        // create the sender
        ServiceBusSender sender = client.CreateSender(topicName);

        var jsonMessage = JsonConvert.SerializeObject(message);

        // create a message that we can send. UTF-8 encoding is used when providing a string.
        ServiceBusMessage finalMessage = new ServiceBusMessage(Encoding.UTF8.GetBytes(jsonMessage));

        // send the message
        await sender.SendMessageAsync(finalMessage);
    }
}
