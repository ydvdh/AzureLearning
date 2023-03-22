using Azure.Storage.Blobs;
using AzureFunctionWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Diagnostics;

namespace AzureFunctionWeb.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        static readonly HttpClient _httpClient = new HttpClient();
        private readonly BlobServiceClient _blobServiceClient;

        public HomeController(ILogger<HomeController> logger, BlobServiceClient blobServiceClient)
        {
            _logger = logger;
            _blobServiceClient = blobServiceClient;
        }

        public IActionResult Index()
        {
            return View();
        }
        // http://localhost:7062/api/OnSalesUploadWritetoQueue
        [HttpPost]
        public async Task<IActionResult> Index(SalesRequest salesRequest, IFormFile file)
        {
            salesRequest.Id = Guid.NewGuid().ToString();

            using (var content = new StringContent(JsonConvert.SerializeObject(salesRequest),
               System.Text.Encoding.UTF8, "application/json"))
            {
                // Call out function here and pass the content
                HttpResponseMessage httpResponse = await _httpClient.PostAsync("http://localhost:7062/api/OnSalesUploadWritetoQueue", content);
                string returnValue = httpResponse.Content.ReadAsStringAsync().Result;
            }
            if(file != null)
            {
                var fileName = salesRequest.Id + Path.GetExtension(file.FileName);
                BlobContainerClient blobContainerClient = _blobServiceClient.GetBlobContainerClient("functionsalereps");
                var blobClient = blobContainerClient.GetBlobClient(fileName);

                var httpHeaders = new Azure.Storage.Blobs.Models.BlobHttpHeaders()
                {
                    ContentType = file.ContentType
                };

                await blobClient.UploadAsync(file.OpenReadStream(), httpHeaders);

                return View();
            }
            return RedirectToAction("Index");
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