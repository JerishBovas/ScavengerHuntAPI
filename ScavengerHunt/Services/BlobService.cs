using Azure.Storage.Blobs;

namespace ScavengerHunt.Services;

public class BlobService : IBlobService
{
    private readonly BlobServiceClient blobServiceClient;
    private readonly IConfiguration configuration;

    public BlobService(BlobServiceClient blobServiceClient, IConfiguration configuration)
    {
        this.blobServiceClient = blobServiceClient;
        this.configuration = configuration;
    }

    public async Task<string> SaveImage(IFormFile file)
    {
        string container = configuration["imgContainer"];
        var blobContainer = blobServiceClient.GetBlobContainerClient(container);
        await blobContainer.CreateIfNotExistsAsync(Azure.Storage.Blobs.Models.PublicAccessType.Blob);

        string[] extList = file.FileName.Split(".");
        string ext = extList.Length > 1 ? "."+ extList.Last() : "";
        string name = Guid.NewGuid() + DateTime.Now.ToBinary().ToString();

        var blobClient = blobContainer.GetBlobClient($"{name}{ext}");

        await blobClient.UploadAsync(file.OpenReadStream(), true);

        return blobClient.Uri.AbsoluteUri.ToString();
    }

    public void DeleteImage(string name)
    {
        string container = configuration["imgContainer"];
        var blobContainer = blobServiceClient.GetBlobContainerClient(container);

        blobContainer.GetBlobClient(name).DeleteIfExists();
    }
}