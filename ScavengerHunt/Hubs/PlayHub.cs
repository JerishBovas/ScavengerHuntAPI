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
    private readonly ILogger<PlayHub> logger;

    public PlayHub(IGameService gameService, IUserService userService, ILogger<PlayHub> logger, IGamePlayService gamePlayService)
    {
        this.gameService = gameService;
        this.userService = userService;
        this.gamePlayService = gamePlayService;
        this.logger = logger;
    }

    public async Task StartGame(Guid gameId)
    {
        if (Guid.TryParse(Context.UserIdentifier, out Guid userId))
        {
            var game = await gameService.GetAsync(gameId, userId);
            if (game is null) { await Clients.Caller.SendAsync("Error", "Game not found!"); return; }
            GamePlay score = new()
            {
                UserId = userId,
                GameEnded = false,
                GameId = gameId,
                GameName = game.Name,
                NoOfItems = game.Items.Count,
                ItemsLeftToFind = new(),
                Score = 0,
                StartTime = DateTimeOffset.Now,
                EndTime = null
            };

            try
            {
                await gamePlayService.CreateAsync(score);
                await gamePlayService.SaveChangesAsync();
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                return;
            }

            GamePlayDto scoreDto = new()
            {
                Id = score.Id,
                GameId = score.GameId,
                GameName = score.GameName,
                NoOfItems = score.NoOfItems,
                Score = score.Score,
                StartTime = score.StartTime,
                EndTime = score.EndTime
            };
            await Clients.Caller.SendAsync("StartGame", scoreDto);
        }
        else
        {
            await Clients.Caller.SendAsync("Error", "Game not found!");
        }
    }

    public async Task VerifyItem()
    {
        Console.WriteLine("Item Verified");
        logger.LogInformation("Item Verified");
        await Clients.Caller.SendAsync("VerifyItem", Context.UserIdentifier?.ToString());
    }
}