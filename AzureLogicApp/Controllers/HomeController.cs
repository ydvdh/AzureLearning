using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using AzureLogicApp.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Text;

namespace AzureLogicApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        static readonly HttpClient client = new HttpClient();
        private readonly BlobServiceClient _blobClient;
        public HomeController(ILogger<HomeController> logger, BlobServiceClient blobClient)
        {
            _logger = logger;
            _blobClient = blobClient;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(SpookyRequest spookyRequest, IFormFile file)
        {
            spookyRequest.Id = Guid.NewGuid().ToString();
            var jsonContent = JsonConvert.SerializeObject(spookyRequest);
            using (var content = new StringContent(jsonContent, Encoding.UTF8, "application/json"))
            {
                HttpResponseMessage httpResponse = await client.PostAsync("https://schema.management.azure.com/providers/Microsoft.Logic/schemas/2016-06-01/workflowdefinition.json#", content);
            }

            if (file != null)
            {
                var fileName = spookyRequest.Id + Path.GetExtension(file.FileName);
                BlobContainerClient blobContainerClient = _blobClient.GetBlobContainerClient("logicappholder");
                var blobClient = blobContainerClient.GetBlobClient(fileName);

                var httpHeaders = new BlobHttpHeaders()
                {
                    ContentType = file.ContentType
                };
                await blobClient.UploadAsync(file.OpenReadStream(), httpHeaders);
            }

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}