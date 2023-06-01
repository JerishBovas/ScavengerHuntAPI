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

    public async Task<GamePlayDto?> StartGame(Guid gameId, Guid gameUserId)
    {
        try
        {
            bool didParse = Guid.TryParse(Context.UserIdentifier, out Guid userId);
            if(!didParse) {await Clients.Caller.SendAsync("Error", "You are not authorized!"); return null;}

            var game = await gameService.GetAsync(gameId, gameUserId);
            if (game is null) { await Clients.Caller.SendAsync("Error", "Game not found!"); return null; }
            
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
            return mapper.Map<GamePlayDto>(play);
        }
        catch (Exception e)
        {
            logger.LogError(e.Message);
            return null;
        }
    }

    public async Task<GamePlayDto?> StopGame(Guid gamePlayId)
    {
        try
        {
            bool didParse = Guid.TryParse(Context.UserIdentifier, out Guid userId);
            if(!didParse) {await Clients.Caller.SendAsync("Error", "You are not authorized!"); return null;}

            var gamePlay = await gamePlayService.GetAsync(gamePlayId, userId);
            if (gamePlay is null) { await Clients.Caller.SendAsync("Error", "Game Play not found!"); return null; }

            gamePlay.GameEnded = true;
            gamePlay.EndTime = DateTimeOffset.UtcNow;
            await gamePlayService.SaveChangesAsync();
            return mapper.Map<GamePlayDto>(gamePlay);
        }
        catch (Exception e)
        {
            logger.LogError(e.Message);
            return null;
        }
    }

    public async Task<VerifiedItemDto?> VerifyImage(ImageData imageData)
    {
        try
        {
            bool didParse = Guid.TryParse(Context.UserIdentifier, out Guid userId);
            if(!didParse) { await Clients.Caller.SendAsync("Error", "You are not authorized!"); return null;}

            bool diditemParse = Guid.TryParse(imageData.ItemId, out Guid itemId);
            if(!diditemParse) { await Clients.Caller.SendAsync("Error", "You are not authorized!"); return null;}

            bool didgameplayParse = Guid.TryParse(imageData.GamePlayId, out Guid gamePlayId);
            if(!didgameplayParse) { await Clients.Caller.SendAsync("Error", "You are not authorized!"); return null;}

            var gamePlay = await gamePlayService.GetAsync(gamePlayId, userId);
            if(gamePlay is null) { await Clients.Caller.SendAsync("Error", "Game Session not found!"); return null;}

            if(gamePlay.StartTime.AddMinutes(gamePlay.GameDuration) < DateTimeOffset.UtcNow)
            {
                gamePlay.EndTime = gamePlay.StartTime.AddMinutes(gamePlay.GameDuration);
                gamePlay.GameEnded = true;
                await gamePlayService.SaveChangesAsync();
                await Clients.Caller.SendAsync("Error", "Game Ended."); 
                return new VerifiedItemDto(gamePlay.GameEnded, null, gamePlay.Score);
            }

            var item = gamePlay.Items.First(x => x.Id == itemId);

            ImageAnalysis result1 = await AnalyzeImageStream(imageData.ImageBytes);
            ImageAnalysis result2 = await AnalyzeImageURL(item.ImageUrl);

            if(AreResultsSimilar(result1, result2))
            {
                gamePlay.ItemsFound.Add(item.Id.ToString());
                gamePlay.Score += 10;
                await gamePlayService.SaveChangesAsync();
                return new VerifiedItemDto(gamePlay.GameEnded, item.Id, gamePlay.Score);
            }
            return new VerifiedItemDto(gamePlay.GameEnded, null, gamePlay.Score);
        }
        catch(Exception e)
        {
            logger.LogError(e.Message);
            await Clients.Caller.SendAsync("Error", "Internal error occured.");
            return null;
        }
    }

    public async Task<bool> GameStatus(string gameId, string userId)
    {
        bool didParseGameId = Guid.TryParse(gameId, out Guid parsedGameId);
        if(!didParseGameId) { return false;}

        bool didParseUserId = Guid.TryParse(userId, out Guid parsedUserId);
        if(!didParseUserId) { return false;}

        bool didParse = Guid.TryParse(Context.UserIdentifier, out Guid realUserId);
        if(!didParse) { return false;}

        var game = await gameService.GetAsync(parsedGameId, parsedUserId);
        if (game is null) { return false; }

        if(game.IsPrivate && game.UserId != realUserId){return false;}

        return true;
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
        // Compare labels
        bool labelsMatch = result1.Description.Tags.SequenceEqual(result2.Description.Tags);

        // Compare confidence scores
        bool confidenceScoresAboveThreshold = result1.Description.Captions.Select(c => c.Confidence).Max() > threshold &&
            result2.Description.Captions.Select(c => c.Confidence).Max() > threshold;

        // Combine criteria (e.g., both labels match and confidence scores are above threshold)
        bool isSimilar = labelsMatch && confidenceScoresAboveThreshold;

        return isSimilar;
    }
}