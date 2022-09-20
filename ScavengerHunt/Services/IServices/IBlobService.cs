namespace ScavengerHunt.Services;

public interface IBlobService
{
    Task<string> SaveImage(IFormFile file);

    void DeleteImage(string name);
}