using Azure.Storage.Blobs;

namespace ScavengerHunt.Services;

public class BlobService : IBlobService
{
    private readonly BlobServiceClient blobServiceClient;

    public BlobService(BlobServiceClient blobServiceClient)
    {
        this.blobServiceClient = blobServiceClient;
    }

    public async Task<string> SaveImage(string container, IFormFile file, string name)
    {
        var blobContainer = blobServiceClient.GetBlobContainerClient(container);
        await blobContainer.CreateIfNotExistsAsync(Azure.Storage.Blobs.Models.PublicAccessType.Blob);

        string[] extList = file.FileName.Split(".");
        string ext = extList.Length > 1 ? "."+ extList.Last() : "";

        var blobClient = blobContainer.GetBlobClient($"{name}{ext}");

        await blobClient.UploadAsync(file.OpenReadStream(), false);

        return blobClient.Uri.AbsoluteUri.ToString();
    }

    public void DeleteImage(string container, string name)
    {
        var blobContainer = blobServiceClient.GetBlobContainerClient(container);

        blobContainer.GetBlobClient(name).DeleteIfExists();
    }
}