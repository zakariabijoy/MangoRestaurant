using Mango.MessageBus;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System.Text;

namespace Mango.Services.ShoppingCartAPI.RabbitMQSender;

public class RabbitMQCartMessageSender : IRabbitMQCartMessageSender
{
    private readonly string _hostname;
    private readonly string _password;
    private readonly string _username;
    private IConnection _connection;

    public RabbitMQCartMessageSender()
    {
        _hostname = "localhost";
        _username = "guest";
        _password = "guest";

    }
    public void SendMessage(BaseMessage message, string queueName)
    {
        var connectionFactory = new ConnectionFactory
        {
            HostName = _hostname,
            UserName = _username,
            Password = _password,
        }; 

        _connection = connectionFactory.CreateConnection();

        using var channel = _connection.CreateModel();
        channel.QueueDeclare(queue: queueName, false, false, false, null);

        var json = JsonConvert.SerializeObject(message);
        var body = Encoding.UTF8.GetBytes(json);

        channel.BasicPublish(exchange:"", routingKey:queueName, basicProperties:null, body:body);
        channel.Close();    
    }
}
