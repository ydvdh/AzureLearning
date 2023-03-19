using AzureBlob.Models;
using AzureBlob.Services;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace AzureBlob.Controllers
{
    public class HomeController : Controller
    {
        private readonly IContainerService _containerService;
        private readonly IBlobService _blobService;

        public HomeController(IContainerService containerService, IBlobService blobService)
        {
            _containerService = containerService;
            _blobService = blobService;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _containerService.GetAllContainerAndBlobs());
        }

        public async Task<IActionResult> Images()
        {
            return View(await _blobService.GetAllBlobsWithUri("azurelearningstoreimages"));
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}