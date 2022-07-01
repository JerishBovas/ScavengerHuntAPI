namespace ScavengerHunt.Services;

public interface IBlobService
{
    Task<string> SaveImage(string container, IFormFile file, string name);
}