namespace ScavengerHunt.Services;

public interface IBlobService
{
    Task<string> UploadImage(string container, string name, Stream file);

    void DeleteImage(string container, string name);
}