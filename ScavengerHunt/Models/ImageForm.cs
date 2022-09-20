namespace ScavengerHunt.Models;

// This Object is used for getting image from api call using multiform format
// This is used when sending images data through api call
public record ImageForm
{
    public IFormFile? ImageFile {get; set;}
}