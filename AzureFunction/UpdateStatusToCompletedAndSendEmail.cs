using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AzureFunction.Data;
using AzureFunction.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using SendGrid.Helpers.Mail;

namespace AzureFunction
{
    public class UpdateStatusToCompletedAndSendEmail
    {
        private readonly AzureFunctionDbContext _db;
        public UpdateStatusToCompletedAndSendEmail(AzureFunctionDbContext db)
        {
            _db = db;
        }

        [FunctionName("UpdateStatusToCompletedAndSendEmail")]
        public async Task Run([TimerTrigger("0 */5 * * * *")]TimerInfo myTimer,
            [SendGrid(ApiKey = "CustomSendGridKeyAppSettingName")] IAsyncCollector<SendGridMessage> messageCollector,
            ILogger log)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");

            IEnumerable<SalesRequest> salesRequestFromDb = _db.SalesRequests.Where(u => u.Status == "Image Processed");
            foreach (var salesReq in salesRequestFromDb)
            {
                //for each request update status
                salesReq.Status = "Completed";
            }

            _db.UpdateRange(salesRequestFromDb);
            _db.SaveChanges();

            var message = new SendGridMessage();
            message.AddTo("dhakal@gmail.com");
            message.AddContent("text/html", $"Processing completed for {salesRequestFromDb.Count()} records");
            message.SetFrom(new EmailAddress("hello@gmail.com"));
            message.SetSubject("Azure Web Processing Successful");
            await messageCollector.AddAsync(message);
        }
    }
}
