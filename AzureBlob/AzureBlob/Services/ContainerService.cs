using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace AzureBlob.Services
{
    public class ContainerService : IContainerService
    {
        private readonly BlobServiceClient _blobServiceClient;

        public ContainerService(BlobServiceClient blobServiceClient)
        {
            _blobServiceClient = blobServiceClient;
        }
        public async Task<List<string>> GetAllContainer()
        {
            List<string> containerNames = new ();
            await foreach (BlobContainerItem blobContainerItem in _blobServiceClient.GetBlobContainersAsync()) 
            {
                containerNames.Add (blobContainerItem.Name);
            }
            return containerNames;
        }
        public async Task<List<string>> GetAllContainerAndBlobs()
        {
            List<string> containerAndBlobNames = new();
            containerAndBlobNames.Add("Account Name : " + _blobServiceClient.AccountName);
            containerAndBlobNames.Add("------------------------------------------------------------------------------------------------------------");
            await foreach (BlobContainerItem blobContainerItem in _blobServiceClient.GetBlobContainersAsync())
            {
                containerAndBlobNames.Add("--" + blobContainerItem.Name);
                BlobContainerClient _blobContainer =
                      _blobServiceClient.GetBlobContainerClient(blobContainerItem.Name);
                await foreach (BlobItem blobItem in _blobContainer.GetBlobsAsync())
                {
                    //get metadata
                    var blobClient = _blobContainer.GetBlobClient(blobItem.Name);
                    BlobProperties blobProperties = await blobClient.GetPropertiesAsync();
                    string blobToAdd = blobItem.Name;
                    if (blobProperties.Metadata.ContainsKey("title"))
                    {
                        blobToAdd += "(" + blobProperties.Metadata["title"] + ")";
                    }

                    containerAndBlobNames.Add("------" + blobToAdd);
                }
                containerAndBlobNames.Add("------------------------------------------------------------------------------------------------------------");

            }
            return containerAndBlobNames;
        }
        public async Task CreateContainer(string containerName)
        {
            BlobContainerClient blobContainerClient = _blobServiceClient.GetBlobContainerClient(containerName);
            await blobContainerClient.CreateIfNotExistsAsync(PublicAccessType.BlobContainer);
        }

        public async Task DeleteContainer(string containerName)
        {
            BlobContainerClient blobContainerClient = _blobServiceClient.GetBlobContainerClient(containerName);
            await blobContainerClient.DeleteIfExistsAsync();
        }     
    }
}
