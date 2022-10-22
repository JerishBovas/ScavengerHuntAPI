using Amazon.Rekognition;
using Amazon.Rekognition.Model;

namespace ScavengerHunt.Services;
public class ClassificationService : IClassificationService
{
    private readonly IConfiguration configuration;
    private readonly AmazonRekognitionClient rekognitionClient;

    public ClassificationService(IConfiguration configuration)
    {
        this.configuration = configuration;
        rekognitionClient = new AmazonRekognitionClient(configuration["AWS:AccessKeyId"], configuration["AWS:SecretAccessKey"], Amazon.RegionEndpoint.USEast2);
    }

    public async Task<DetectLabelsResponse> DetectLabels(DetectLabelsRequest detectlabelsRequest)
    {
        return await rekognitionClient.DetectLabelsAsync(detectlabelsRequest);
    }
}