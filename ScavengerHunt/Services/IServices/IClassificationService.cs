using System;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;

namespace ScavengerHunt.Services;
public interface IClassificationService
{
    public Task<ImageAnalysis> AnalyzeImageInStreamAsync(Stream image, List<VisualFeatureTypes?>? visualFeatureTypes = null);
    
    public Task<ImageAnalysis> AnalyzeImageAsync(string imageUrl, List<VisualFeatureTypes?>? visualFeatureTypes = null);
}