using Amazon.Rekognition.Model;

namespace ScavengerHunt.Models;

public struct ImageLabels
{
    public string Url { get; set; }
    public List<Tag> Labels { get; set; }
}

public struct Tag
{
    public string Label { get; set; }
    public double Confidence { get; set; }
    public BoundingBox BoundingBox { get; set; }
}