using Azure.Messaging.ServiceBus;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace ChapterBlobProcessorAzureFunctionApp
{
    public class ChapterBlobProcessorAzureFunction
    {
        private readonly ILogger<ChapterBlobProcessorAzureFunction> _logger;
        private readonly ServiceBusSender _serviceBusSender;
        public ChapterBlobProcessorAzureFunction(ILogger<ChapterBlobProcessorAzureFunction> logger, 
            ServiceBusSender serviceBusSender)
        {
            _logger = logger;
            _serviceBusSender = serviceBusSender;
        }

        [Function(nameof(ChapterBlobProcessorAzureFunction))]
        public async Task Run(
            [BlobTrigger("chapter-azure-blob-storage/{name}", Connection = "AzureWebJobsStorage")] Stream stream, 
            string name)
        {
            using var blobStreamReader = new StreamReader(stream);
            var content = await blobStreamReader.ReadToEndAsync();
            _logger.LogInformation($"C# Blob trigger function Processed blob\n Name: {name} \n Data: {content}");

            await _serviceBusSender.SendMessagesAsync([new ServiceBusMessage($"File with name: {name} was processed!")]);
        }
    }
}
