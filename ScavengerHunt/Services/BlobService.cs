using Azure.Storage.Blobs;

namespace ScavengerHunt.Services;

public class BlobService : IBlobService
{
    private readonly BlobServiceClient blobServiceClient;

    public BlobService(BlobServiceClient blobServiceClient)
    {
        this.blobServiceClient = blobServiceClient;
    }

    // Uploads the given Stream file to the azure storage
    // Created image name using guid and binary time
    // returns the absolute path of the image file
    public async Task<string> UploadImage(string container, string name, Stream file)
    {
        var blobContainer = blobServiceClient.GetBlobContainerClient(container);
        await blobContainer.CreateIfNotExistsAsync(Azure.Storage.Blobs.Models.PublicAccessType.Blob);
        var blobClient = blobContainer.GetBlobClient(name);

        await blobClient.UploadAsync(file, true);

        return blobClient.Uri.AbsoluteUri.ToString();
    }

    // Deletes the image blob in the given url
    public void DeleteImage(string container, string name)
    {
        var blobContainer = blobServiceClient.GetBlobContainerClient(container);

        blobContainer.GetBlobClient(name).DeleteIfExists();
    }
}