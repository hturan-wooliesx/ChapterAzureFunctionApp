using Azure.Messaging.ServiceBus;
using ChapterAzureFunctionApp.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using System.Net;

namespace ChapterAzureFunctionApp
{
    public class ChapterAzureFunctions(ILoggerFactory loggerFactory, 
        ServiceBusSender serviceBusSender,
        ServiceBusReceiver serviceBusReceiver)
    {
        private readonly ILogger _logger = loggerFactory.CreateLogger<ChapterAzureFunctions>();
        private readonly ServiceBusSender _serviceBusSender = serviceBusSender;
        private readonly ServiceBusReceiver _serviceBusReceiver = serviceBusReceiver;

        [Function("ChapterAzureFunction")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequest req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            req.Query.TryGetValue("name", out var nameQueryParam);

            var nameParam = nameQueryParam.FirstOrDefault();

            return new OkObjectResult(
                string.IsNullOrWhiteSpace(nameParam) ? "Welcome to Azure Functions!" : $"Welcome to Azure Functions {nameParam}!");
        }

        [Function("ChapterAzureFunctionWithOutputBinding")]
        public async Task<MultiResponse> RunWithOutputBinding([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")] HttpRequestData req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            var response = req.CreateResponse(HttpStatusCode.OK);
            response.Headers.Add("Content-Type", "text/plain; charset=utf-8");

            var message = "Welcome to Azure Functions!";
            await response.WriteStringAsync(message);

            return new MultiResponse
            {
                Messages = [message],
                HttpResponse = response
            };
        }


        /// <summary>
        /// Azure function with Blob trigger that runs a blob is uploaded to "chapter-azure-blob-storage" 
        /// and creates a message on Service Bus queue (q2)
        /// </summary>
        /// <param name="blob"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        [Function(nameof(ChapterBlobProcessorAzureFunction))]
        public async Task<MultiResponse> ChapterBlobProcessorAzureFunction(
            [BlobTrigger("chapter-azure-blob-storage/{name}", Connection = "AzureWebJobsStorage")] byte[] blob,
            string name)
        {
            using var blobMemoryStream = new MemoryStream(blob);
            var blobStreamReader = new StreamReader(blobMemoryStream); 
            var content = await blobStreamReader.ReadToEndAsync();

            _logger.LogInformation("C# Blob trigger function Processed blob\n Name: {Name} \n Content Lenght: {ContentLength}", name, content.Length);

            await _serviceBusSender.SendMessagesAsync([new ServiceBusMessage($"File with name: {name} was processed!")]);

            return new MultiResponse
            {
                Messages = [$"File with name: {name} was processed!"],
                HttpResponse = null
            };
        }

        /// <summary>
        /// Azure function with timer trigger that runs every 1 minute
        /// </summary>
        /// <param name="myTimer"></param>
        /// <returns></returns>
        [Function(nameof(ChapterTimerTriggeredAzureFunction))]
        public async Task<MultiResponse> ChapterTimerTriggeredAzureFunction([TimerTrigger("* * * * *", RunOnStartup = true)] TimerInfo myTimer)
        {
            _logger.LogInformation("ChapterTimerTriggeredAzureFunction executed at: {TImeStamp}", DateTime.Now);

            if (myTimer.ScheduleStatus is not null)
            {
                _logger.LogInformation("Next timer schedule at: {NextTimeStamp}", myTimer.ScheduleStatus.Next);
            }

            return new MultiResponse
            {
                Messages = [$"Timer triggered azure funcion called! @{DateTime.Now}"]
            };
        }


        /// <summary>
        /// Azure function with timer trigger that runs every 5 minutes to process messages in Service Bus queue (q2)
        /// </summary>
        /// <param name="myTimer"></param>
        /// <returns></returns>
        [Function(nameof(ChapterTimerTriggeredAzureFunction2))]
        public async Task<MultiResponse> ChapterTimerTriggeredAzureFunction2([TimerTrigger("*/5 * * * *", RunOnStartup = true)] TimerInfo myTimer)
        {
            _logger.LogInformation("ChapterTimerTriggeredAzureFunction2 executed at: {TImeStamp}", DateTime.Now);

            var messages = await _serviceBusReceiver.ReceiveMessagesAsync(10);

            string[] responseMessages = new string[messages.Count];

            for (int i = 0; i < messages.Count; i++)
            {
                var serviceBusReceivedMessage = messages[i];
                var messageContent = serviceBusReceivedMessage.Body.ToString() ?? string.Empty;

                responseMessages[i] = $"Service Bus message with ID {serviceBusReceivedMessage.MessageId} processed at {DateTime.Now}. Message content: {messageContent}";

                await _serviceBusReceiver.CompleteMessageAsync(serviceBusReceivedMessage);
            }

            return new MultiResponse
            {
                Messages = responseMessages
            };
        }
    }
}
