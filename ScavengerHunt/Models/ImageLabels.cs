using Amazon.Rekognition.Model;

namespace ScavengerHunt.Models;

public struct ImageLabels
{
    public string Url { get; set; }
    public List<Label> Labels { get; set; }
}