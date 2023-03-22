using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AzureFunction.Data;
using AzureFunction.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace AzureFunction
{
    public class GroceryAPI
    {
        private readonly ILogger<GroceryAPI> _logger;
        private readonly AzureFunctionDbContext _dbContext;

        public GroceryAPI(ILogger<GroceryAPI> log, AzureFunctionDbContext dbContext)
        {
            _logger = log;
            _dbContext = dbContext;
        }

        [FunctionName("CreateGrocery")]
        public async Task<IActionResult> CreateGrocery(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "GroceryList")] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("Creating Grocery List Item.");


            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            GroceryItem_Upsert data = JsonConvert.DeserializeObject<GroceryItem_Upsert>(requestBody);

            var groceryItem = new GroceryItem
            {
                Name = data.Name
            };

            _dbContext.GroceryItems.Add(groceryItem);
            _dbContext.SaveChanges();

            return new OkObjectResult(groceryItem);
        }
        [FunctionName("GetGrocery")]
        public async Task<IActionResult> GetGrocery(
           [HttpTrigger(AuthorizationLevel.Function, "get", Route = "GroceryList")] HttpRequest req,
           ILogger log)
        {
            log.LogInformation("Getting all Grocery List Item.");

            return new OkObjectResult(_dbContext.GroceryItems.ToList());
        }

        [FunctionName("GetGroceryById")]
        public async Task<IActionResult> GetGroceryById(
         [HttpTrigger(AuthorizationLevel.Function, "get", Route = "GroceryList/{id}")] HttpRequest req,
         ILogger log, string id)
        {
            log.LogInformation("Getting Grocery List Item by ID.");
            var item = _dbContext.GroceryItems.FirstOrDefault(u => u.Id == id);
            if (item == null)
            {
                return new NotFoundResult();
            }

            return new OkObjectResult(item);
        }


        [FunctionName("UpdateGrocery")]
        public async Task<IActionResult> UpdateGrocery(
           [HttpTrigger(AuthorizationLevel.Function, "put", Route = "GroceryList/{id}")] HttpRequest req,
           ILogger log, string id)
        {
            log.LogInformation("Updatind Grocery List Item.");
            var item = _dbContext.GroceryItems.FirstOrDefault(u => u.Id == id);
            if (item == null)
            {
                return new NotFoundResult();
            }

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            GroceryItem_Upsert updatedData = JsonConvert.DeserializeObject<GroceryItem_Upsert>(requestBody);

            if (!string.IsNullOrEmpty(updatedData.Name))
            {
                item.Name = updatedData.Name;
                _dbContext.GroceryItems.Update(item);
                _dbContext.SaveChanges();
            }

            return new OkObjectResult(item);
        }

        [FunctionName("DeleteGrocery")]
        public async Task<IActionResult> DeleteGrocery(
           [HttpTrigger(AuthorizationLevel.Function, "delete", Route = "GroceryList/{id}")] HttpRequest req,
           ILogger log, string id)
        {
            log.LogInformation("Delete Grocery List Item.");


            var item = _dbContext.GroceryItems.FirstOrDefault(u => u.Id == id);
            if (item == null)
            {
                return new NotFoundResult();
            }
            _dbContext.GroceryItems.Remove(item);
            _dbContext.SaveChanges();

            return new OkResult();
        }
    }
}

