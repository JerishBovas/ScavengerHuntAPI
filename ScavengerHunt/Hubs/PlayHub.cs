using AutoMapper;
using Microsoft.AspNetCore.SignalR;
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

    public async Task StartGame(Guid gameId)
    {
        try
        {
            bool didParse = Guid.TryParse(Context.UserIdentifier, out Guid userId);
            if(!didParse) {await Clients.Caller.SendAsync("Error", "You are not authorized!"); return;}

            var game = await gameService.GetAsync(gameId, userId);
            if (game is null) { await Clients.Caller.SendAsync("Error", "Game not found!"); return; }

            if(!game.IsReadyToPlay)
            {
                await Clients.Caller.SendAsync("Error", "Game under maintenance. Please try again later.");
                return;
            }

            GamePlay score = new()
            {
                GameId = gameId,
                UserId = userId,
                GameName = game.Name,
                NoOfItems = game.Items.Count,
                ItemsLeftToFind = game.Items.ToList()
            };

            await gamePlayService.CreateAsync(score);
            await gamePlayService.SaveChangesAsync();
            await Clients.Caller.SendAsync("StartGame", mapper.Map<GamePlay, GamePlayDto>(score));
        }
        catch (Exception e)
        {
            logger.LogError(e.Message);
            return;
        }
    }

    public async Task CheckImage(Guid gamePlayId, string base64Item)
    {
        try
        {
            bool didParse = Guid.TryParse(Context.UserIdentifier, out Guid userId);
            if(!didParse) { await Clients.Caller.SendAsync("Error", "You are not authorized!"); return;}

            var gamePlay = await gamePlayService.GetAsync(gamePlayId, userId);
            if(gamePlay is null) { await Clients.Caller.SendAsync("Error", "Game not found!"); return;}

            if(gamePlay.StartTime.AddMinutes(gamePlay.GameDuration) < DateTimeOffset.UtcNow)
            {
                gamePlay.EndTime = gamePlay.StartTime.AddMinutes(gamePlay.GameDuration);
                await Clients.Caller.SendAsync("Error", "Game Ended."); return;
            }
        }
        catch(Exception e)
        {
            logger.LogError(e.Message);
            await Clients.Caller.SendAsync("Error", "Internal error occured.");
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
}