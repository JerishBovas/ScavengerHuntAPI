using Microsoft.AspNetCore.SignalR;
using ScavengerHunt.DTOs;
using ScavengerHunt.Services;

// This hub controlls the items part of games
// When user starts adding , editing or deleting items, this hub activates
// Users can lively do CRUD action on items.
namespace ScavengerHunt.Hubs;
public class ItemsHub : Hub
{
    private readonly IGameService gameService;
    private readonly ILogger<ItemsHub> logger;

    public ItemsHub(IGameService gameService, ILogger<ItemsHub> logger)
    {
        this.gameService = gameService;
        this.logger = logger;
    }

    public async Task AddItem(Guid gameId, ItemCreateDto item, string base64Item)
    {
        
    }
}