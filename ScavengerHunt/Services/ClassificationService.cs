using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;

namespace ScavengerHunt.Services;
public class ClassificationService : IClassificationService
{
    private readonly IConfiguration configuration;
    private readonly ComputerVisionClient client;

    public ClassificationService(IConfiguration configuration)
    {
        this.configuration = configuration;
        client = new ComputerVisionClient(new ApiKeyServiceClientCredentials(configuration["VISION_KEY"]))
        {
            Endpoint = configuration["VISION_ENDPOINT"]
        };
    }

    public async Task<ImageAnalysis> AnalyzeImageInStreamAsync(Stream image, List<VisualFeatureTypes?>? visualFeatureTypes)
    {
        return await client.AnalyzeImageInStreamAsync(image, visualFeatureTypes);
    }

    public async Task<ImageAnalysis> AnalyzeImageAsync(string imageUrl, List<VisualFeatureTypes?>? visualFeatureTypes)
    {
        return await client.AnalyzeImageAsync(imageUrl, visualFeatureTypes);
    }
}