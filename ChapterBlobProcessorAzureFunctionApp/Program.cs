using Azure.Messaging.ServiceBus;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
    .ConfigureServices(services =>
    {
        services.AddApplicationInsightsTelemetryWorkerService();
        services.ConfigureFunctionsApplicationInsights();

        services.AddSingleton<ServiceBusSender>(provider =>
        {
            var connString = "Endpoint=sb://wow-d-aae-dx-sb-services.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=FX2ji7Gx/OJsyuuYdFFUE0E/jv0zBw6OxswgwePWseg=;TransportType=NetMessaging";
            var serviceBusClient = new ServiceBusClient(connString);

            var sender = serviceBusClient.CreateSender("q2");

            return sender;
        });
    })
    .Build();

host.Run();