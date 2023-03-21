using System;
using AzureFunction.Data;
using AzureFunction.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace AzureFunction
{
    public class OnQueueTriggerUpdateDatabase
    {
        private readonly AzureFunctionDbContext _dbContext ;

        public OnQueueTriggerUpdateDatabase(AzureFunctionDbContext dbContext)
        {
            _dbContext = dbContext ;
        }

        [FunctionName("OnQueueTriggerUpdateDatabase")]
        public void Run([QueueTrigger("SalesRequestInBound", Connection = "AzureWebJobsStorage")]SalesRequest myQueueItem, ILogger log)
        {
            log.LogInformation($"C# Queue trigger function processed: {myQueueItem}");

            myQueueItem.Status = "Submitted";
            _dbContext.SalesRequests.Add(myQueueItem);
            _dbContext.SaveChanges();
        }
    }
}
