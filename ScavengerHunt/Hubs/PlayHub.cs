using System.Text.Json;
using AutoMapper;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;
using ScavengerHunt.DTOs;
using ScavengerHunt.Models;
using ScavengerHunt.Services;

// This hub controls all the game play
// Once the user presses a game to play, this hub gets activated
// After this point everything becomes live.
namespace ScavengerHunt.Hubs;
public class PlayHub : Hub
{
    private readonly IGameService gameService;
    private readonly IUserService userService;
    private readonly IGamePlayService gamePlayService;
    private readonly IClassificationService classificationService;
    private readonly ILogger<PlayHub> logger;
    private readonly IMapper mapper;

    public PlayHub(IGameService gameService, IUserService userService, ILogger<PlayHub> logger, IGamePlayService gamePlayService, IMapper mapper, IClassificationService classificationService)
    {
        this.gameService = gameService;
        this.userService = userService;
        this.gamePlayService = gamePlayService;
        this.classificationService = classificationService;
        this.logger = logger;
        this.mapper = mapper;
    }

    public async Task StartGame(Guid gameId, Guid gameUserId)
    {
        try
        {
            bool didParse = Guid.TryParse(Context.UserIdentifier, out Guid userId);
            if(!didParse) {await Clients.Caller.SendAsync("Error", "You are not authorized!"); return;}

            var game = await gameService.GetAsync(gameId, gameUserId);
            if (game is null) { await Clients.Caller.SendAsync("Error", "Game not found!"); return; }
            
            List<Item> items = new List<Item>();

            foreach (var gameItem in game.Items)
            {
                var clonedItem = new Item
                {
                    Id = gameItem.Id,
                    Name = gameItem.Name,
                    ImageUrl = gameItem.ImageUrl
                };

                items.Add(clonedItem);
            }
            GamePlay play = new()
            {
                GameId = gameId,
                Name = game.Name,
                Address = game.Address,
                Country = game.Country,
                UserId = userId,
                Coordinate = new Coordinate{
                    Latitude = game.Coordinate.Latitude, 
                    Longitude = game.Coordinate.Longitude
                },
                Items = items,
                GameDuration = game.GameDuration,
                StartTime = DateTimeOffset.UtcNow,
                Deadline = DateTimeOffset.UtcNow.AddMinutes(game.GameDuration)
            };

            await gamePlayService.CreateAsync(play);
            await gamePlayService.SaveChangesAsync();
            await Clients.Caller.SendAsync("StartGame", mapper.Map<GamePlayDto>(play));
        }
        catch (Exception e)
        {
            logger.LogError(e.Message);
            await Clients.Caller.SendAsync("Error", e.Message);
            return;
        }
    }

    public async Task EndGame(Guid gamePlayId)
    {
        try
        {
            bool didParse = Guid.TryParse(Context.UserIdentifier, out Guid userId);
            if(!didParse) {await Clients.Caller.SendAsync("Error", "You are not authorized!"); return;}

            var gamePlay = await gamePlayService.GetAsync(gamePlayId, userId);
            if (gamePlay is null) { await Clients.Caller.SendAsync("Error", "Game Play not found!"); return; }

            gamePlay.GameEnded = true;
            gamePlay.EndTime = DateTimeOffset.UtcNow;
            await gamePlayService.SaveChangesAsync();
            await Clients.Caller.SendAsync("EndGame", mapper.Map<GamePlayDto>(gamePlay));
        }
        catch (Exception e)
        {
            logger.LogError(e.Message);
            await Clients.Caller.SendAsync("Error", e.Message);
            return;
        }
    }

    public async Task VerifyImage(ImageData imageData)
    {
        try
        {
            logger.LogInformation($"rawItemID: {imageData.ItemId}");
            logger.LogInformation($"rawGamePlayID: {imageData.GamePlayId}");

            if(!Guid.TryParse(Context.UserIdentifier, out Guid userId)){
                logger.LogInformation($"userId: {userId}");
                await Clients.Caller.SendAsync("Error", $"userid: {userId}");
                return;
            }
            if(!Guid.TryParse(imageData.ItemId, out Guid itemId)){
                logger.LogInformation($"itemID: {itemId}");
                await Clients.Caller.SendAsync("Error", $"itemId: {itemId}");
                return;
            }
            if(!Guid.TryParse(imageData.GamePlayId, out Guid gamePlayId)){
                logger.LogInformation($"gamePlayId: {gamePlayId}");
                await Clients.Caller.SendAsync("Error", $"gamePlayId: {gamePlayId}");
                return;
            }

            // if (!Guid.TryParse(Context.UserIdentifier, out Guid userId) || 
            //     !Guid.TryParse(imageData.ItemId, out Guid itemId) || 
            //     !Guid.TryParse(imageData.GamePlayId, out Guid gamePlayId))
            // {
            //     await Clients.Caller.SendAsync("Error", "Internal Error");
            //     return;
            // }

            var gamePlay = await gamePlayService.GetAsync(gamePlayId, userId);
            if(gamePlay is null) { await Clients.Caller.SendAsync("Error", "Game Session not found!"); return;}

            if(gamePlay.StartTime.AddMinutes(gamePlay.GameDuration) < DateTimeOffset.UtcNow)
            {
                gamePlay.EndTime = gamePlay.StartTime.AddMinutes(gamePlay.GameDuration);
                gamePlay.GameEnded = true;
                await gamePlayService.SaveChangesAsync();
                await Clients.Caller.SendAsync("Error", "Game Ended."); 
                await Clients.Caller.SendAsync("VerifyImage", new VerifiedItemDto(gamePlay.GameEnded, null, gamePlay.Score));
            }

            var item = gamePlay.Items.FirstOrDefault(x => x.Id == itemId);
            if (item == null)
            {
                await Clients.Caller.SendAsync("Error", "Item not found!");
                return;
            }

            ImageAnalysis result1 = await AnalyzeImageStream(Convert.FromBase64String(imageData.ImageString));
            ImageAnalysis result2 = await AnalyzeImageURL(item.ImageUrl);

            if(AreResultsSimilar(result1, result2))
            {
                gamePlay.ItemsFound.Add(item.Id.ToString());
                gamePlay.Score += 10;
                await gamePlayService.SaveChangesAsync();
                await Clients.Caller.SendAsync("VerifyImage", new VerifiedItemDto(gamePlay.GameEnded, item.Id, gamePlay.Score));
            }
            await Clients.Caller.SendAsync("VerifyImage", new VerifiedItemDto(gamePlay.GameEnded, null, gamePlay.Score));
        }
        catch(Exception e)
        {
            logger.LogError(e.Message);
            await Clients.Caller.SendAsync("Error", e.Message);
            return;
        }
    }

    public async Task GameStatus(string gameId, string userId)
    {
        if(!Guid.TryParse(gameId, out Guid parsedGameId) ||
        !Guid.TryParse(userId, out Guid parsedUserId) ||
        !Guid.TryParse(Context.UserIdentifier, out Guid realUserId)){
            await Clients.Caller.SendAsync("GameStatus", false); return;
        }

        var game = await gameService.GetAsync(parsedGameId, parsedUserId);
        if (game is null) { 
            await Clients.Caller.SendAsync("GameStatus", false);
            return; }

        if(game.IsPrivate && game.UserId != realUserId){await Clients.Caller.SendAsync("GameStatus", false); return;}

        await Clients.Caller.SendAsync("GameStatus", true);
    }

    private async Task<ImageAnalysis> AnalyzeImageURL(string url)
    {
        // Analyze the image using Azure Cognitive Services
        ImageAnalysis result = await classificationService.AnalyzeImageAsync(url, new List<VisualFeatureTypes?> { VisualFeatureTypes.Description,
            VisualFeatureTypes.Categories,
            VisualFeatureTypes.Tags });

        return result;
    }

    private async Task<ImageAnalysis> AnalyzeImageStream(byte[] imageBytes)
    {
        using (MemoryStream imageStream = new MemoryStream(imageBytes))
        {
            // Analyze the image using Azure Cognitive Services
            ImageAnalysis result = await classificationService.AnalyzeImageInStreamAsync(imageStream, new List<VisualFeatureTypes?> { VisualFeatureTypes.Description,
            VisualFeatureTypes.Categories,
            VisualFeatureTypes.Tags });

            return result;
        }
    }

    private bool AreResultsSimilar(ImageAnalysis result1, ImageAnalysis result2, float threshold = 0.8f)
    {
        // Validate input parameters
        if (result1 == null || result2 == null)
        {
            throw new ArgumentNullException();
        }

        // Compare labels (case-insensitive)
        bool labelsMatch = result1.Description.Tags.SequenceEqual(result2.Description.Tags, StringComparer.OrdinalIgnoreCase);

        // Compare confidence scores
        bool confidenceScoresAboveThreshold = result1.Description.Captions.Any(c => c.Confidence > threshold) &&
            result2.Description.Captions.Any(c => c.Confidence > threshold);

        // Combine criteria (e.g., both labels match and confidence scores are above threshold)
        bool isSimilar = labelsMatch && confidenceScoresAboveThreshold;

        return isSimilar;
    }
}