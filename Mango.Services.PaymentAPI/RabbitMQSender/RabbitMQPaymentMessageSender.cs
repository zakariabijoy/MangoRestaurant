using Mango.MessageBus;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System.Text;

namespace Mango.Services.PaymentAPI.RabbitMQSender;

public class RabbitMQPaymentMessageSender : IRabbitMQPaymentMessageSender
{
    private readonly string _hostname;
    private readonly string _password;
    private readonly string _username;
    private IConnection? _connection;
    private const string ExchangeName = "DirectPaymentUpdate_Exchange"; 
    private const string PaymentEmailUpdateQueueName = "PaymentEmailUpdateQueueName"; 
    private const string PaymentOrderUpdateQueueName = "PaymentOrderUpdateQueueName";

    public RabbitMQPaymentMessageSender()
    {
        _hostname = "localhost";
        _username = "guest";
        _password = "guest";

    }
    public void SendMessage(BaseMessage message)
    {
        if (ConnectionExists())
        {
            using var channel = _connection?.CreateModel();
            channel?.ExchangeDeclare(ExchangeName, ExchangeType.Direct, durable:false);
            channel?.QueueDeclare(PaymentEmailUpdateQueueName, false, false, false, null);
            channel?.QueueDeclare(PaymentOrderUpdateQueueName, false, false, false, null);

            channel?.QueueBind(PaymentEmailUpdateQueueName,ExchangeName,"PaymentEmail");
            channel?.QueueBind(PaymentOrderUpdateQueueName,ExchangeName,"PaymentOrder");

            var json = JsonConvert.SerializeObject(message);
            var body = Encoding.UTF8.GetBytes(json);

            channel?.BasicPublish(exchange: ExchangeName, "PaymentEmail", basicProperties: null, body: body);
            channel?.BasicPublish(exchange: ExchangeName, "PaymentOrder", basicProperties: null, body: body);
        }
          
    }

    private void CreateConnection()
    {
        try
        {
            var connectionFactory = new ConnectionFactory
            {
                HostName = _hostname,
                UserName = _username,
                Password = _password,
            };

            _connection = connectionFactory?.CreateConnection();
        }
        catch (Exception ex)
        {

            //log exception
            Console.WriteLine(ex.Message.ToString());
        }  
    }

    private bool ConnectionExists()
    {
        if(_connection is not null) 
        {
            return true;
        }
        CreateConnection();
        return _connection is not null;
    }
}
