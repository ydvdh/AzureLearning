using System;
using System.IO;
using System.Linq;
using AzureFunction.Data;
using AzureFunction.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace AzureFunction
{
    public class BlobResizeTriggerUpdateStatusInDb
    {
        private readonly AzureFunctionDbContext _db;
        public BlobResizeTriggerUpdateStatusInDb(AzureFunctionDbContext db)
        {
            _db = db;
        }
        [FunctionName("BlobResizeTriggerUpdateStatusInDb")]
        public void Run([BlobTrigger("functionsalesreps-sm/{name}", Connection = "AzureWebJobsStorage")]Stream myBlob, string name, ILogger log)
        {
            log.LogInformation($"C# Blob trigger function Processed blob\n Name:{name} \n Size: {myBlob.Length} Bytes");

            var fileName = Path.GetFileNameWithoutExtension(name);
            SalesRequest salesRequestFromDb = _db.SalesRequests.FirstOrDefault(u => u.Id == fileName);
            if (salesRequestFromDb != null)
            {
                salesRequestFromDb.Status = "Image Processed";
                _db.SalesRequests.Update(salesRequestFromDb);
                _db.SaveChanges();
            }
        }
    }
}
