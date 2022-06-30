using Azure.Storage.Blobs;

namespace ScavengerHunt.Services;

public class BlobService : IBlobService
{
    private readonly BlobServiceClient blobServiceClient;

    public BlobService(BlobServiceClient blobServiceClient)
    {
        this.blobServiceClient = blobServiceClient;
    }
    public async Task<string> SaveImage(string container, IFormFile file, Guid id)
    {
        var blobContainer = blobServiceClient.GetBlobContainerClient(container);
        await blobContainer.CreateIfNotExistsAsync(Azure.Storage.Blobs.Models.PublicAccessType.Blob);

        string ext = file.FileName.Split(".").LastOrDefault() ?? "";

        var blobClient = blobContainer.GetBlobClient($"{id}{ext}");

        await blobClient.UploadAsync(file.OpenReadStream(), true);

        return blobClient.Uri.AbsoluteUri.ToString();
    }
}