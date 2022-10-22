using Amazon.Rekognition.Model;

namespace ScavengerHunt.Services;
public interface IClassificationService
{
    public Task<DetectLabelsResponse> DetectLabels(DetectLabelsRequest detectlabelsRequest);
}