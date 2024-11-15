using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
    .ConfigureServices(services =>
    {
        // connection string to subscription service bus dev instance
        var connString = "Endpoint=sb://wow-d-aae-dx-sb-services.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=FX2ji7Gx/OJsyuuYdFFUE0E/jv0zBw6OxswgwePWseg=;TransportType=NetMessaging";
        var serviceBusClient = new ServiceBusClient(connString);

        services.AddSingleton<ServiceBusReceiver>(provider =>
        {
            var receiver = serviceBusClient.CreateReceiver("q2");

            return receiver;
        });


        services.AddSingleton<ServiceBusSender>(provider =>
        {
            var sender = serviceBusClient.CreateSender("q2");

            return sender;
        });
    })
    .Build();

host.Run();
