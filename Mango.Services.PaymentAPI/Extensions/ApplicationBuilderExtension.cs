using Mango.Services.PaymentAPI.Messaging;

namespace Mango.Services.PaymentAPI.Extensions;

public static class ApplicationBuilderExtension
{
    public static IAzureServiceBusConsumer ServiceBusConsumer { get; set; } 
    public static IApplicationBuilder UseAzureServiceBusConsumer(this IApplicationBuilder app)
    {
        ServiceBusConsumer = app.ApplicationServices.GetService<IAzureServiceBusConsumer>();
        var hostApplicationLife = app.ApplicationServices.GetService<IHostApplicationLifetime>();

        hostApplicationLife.ApplicationStarted.Register(onStart);
        hostApplicationLife.ApplicationStopped.Register(onStop);

        return app;
    }

    private static void onStart()
    {
        ServiceBusConsumer.Start();
    }

    private static void onStop()
    {
        ServiceBusConsumer.Stop();
    }
}
