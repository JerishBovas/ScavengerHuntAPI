using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using ScavengerHunt.DTOs;
using ScavengerHunt.Models;
using ScavengerHunt.Services;

namespace ScavengerHunt.Hubs;

public class PlayHub : Hub
{
    private readonly IGameService gameService;
    private readonly IUserService userService;
    private readonly ILogger<PlayHub> logger;

    public PlayHub(IGameService gameService, IUserService userService, ILogger<PlayHub> logger)
    {
        this.gameService = gameService;
        this.userService = userService;
        this.logger = logger;
    }

    public async Task StartGame(Guid gameId)
    {
        if (Guid.TryParse(Context.ConnectionId, out Guid userId))
        {
            var game = await gameService.GetAsync(userId, gameId);
            var user = await userService.GetAsync(userId);
            if (game is null || user is null) { await Clients.Caller.SendAsync("Error", "Game not found!"); return; }
            GameScore score = new()
            {
                id = Guid.NewGuid(),
                GameEnded = false,
                GameId = gameId,
                GameName = game.Name,
                NoOfItems = game.Items.Count,
                ItemsFound = 0,
                Score = 0,
                StartTime = DateTimeOffset.Now,
                ExpiryTime = DateTimeOffset.Now.AddMinutes(game.Items.Count),
                EndTime = null
            };

            user.UserLog.ScoreLog.Add(score);
            try
            {
                userService.UpdateAsync(user);
                await userService.SaveChangesAsync();
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                return;
            }

            GameScoreDto scoreDto = new()
            {
                id = score.id,
                GameId = score.GameId,
                GameName = score.GameName,
                NoOfItems = score.NoOfItems,
                ItemsFound = score.ItemsFound,
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
        await Clients.Caller.SendAsync("VerifyItem", "Hello its connected");
    }
}