using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

namespace ChapterAzureFunctionApp.Models
{
    public class MultiResponse
    {
        [QueueOutput("chapter-azure-function-queue", Connection = "AzureWebJobsStorage")]
        public string[] Messages { get; set; }
        public HttpResponseData HttpResponse { get; set; }
    }
}
