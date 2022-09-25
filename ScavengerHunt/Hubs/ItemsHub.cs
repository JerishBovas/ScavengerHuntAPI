using Microsoft.AspNetCore.SignalR;
using ScavengerHunt.DTOs;
using ScavengerHunt.Models;
using ScavengerHunt.Services;

// This hub controlls the items part of games
// When user starts adding , editing or deleting items, this hub activates
// Users can lively do CRUD action on items.
namespace ScavengerHunt.Hubs;
public class ItemsHub : Hub
{
    private readonly IGameService gameService;
    private readonly IBlobService blobService;
    private readonly ILogger<ItemsHub> logger;

    public ItemsHub(IGameService gameService, IBlobService blobService, ILogger<ItemsHub> logger)
    {
        this.gameService = gameService;
        this.blobService = blobService;
        this.logger = logger;
    }

    // Adds items to the storage and database
    // Also authenticates the user and game
    public async Task AddItem(Guid gameId, ItemCreateDto itemDto, string base64Item)
    {
        bool didParse = Guid.TryParse(Context.UserIdentifier, out Guid userId);
        if(!didParse)
        {
            await Clients.Caller.SendAsync("Error", "Invalid User Login");
            return;
        }

        try
        {
            var game = await gameService.GetAsync(gameId, userId);
            if(game == null)
            {
                await Clients.Caller.SendAsync("Error", "Either Game doesn't exist or you are not authorized to access");
            }

            byte[] imageArray = Convert.FromBase64String(base64Item);
            string imageName = Guid.NewGuid() + DateTime.Now.ToBinary().ToString() + ".jpg";

            var ms = new MemoryStream(imageArray, false);
            string imageUrl = await blobService.UploadImage("items", imageName, ms);

            Item item = new()
            {
                Name = itemDto.Name,
                Description = itemDto.Description,
                ImageName = imageUrl
            };

            game?.Items.Add(item);
            await gameService.SaveChangesAsync();
        }
        catch(ArgumentNullException ane)
        {
            logger.LogError(ane.Message);
            await Clients.Caller.SendAsync("Error", "One of the information is missing or not valid. Please try again.");
        }
        catch(FormatException fe)
        {
            logger.LogError(fe.Message);
            await Clients.Caller.SendAsync("Error", "The given image is not in the correct format. Please try again with correct image.");
        }
        catch(Exception e)
        {
            logger.LogError(e.Message);
            await Clients.Caller.SendAsync("Error", "An unknown error occured. Please try again later.");
        }
    }

    public async Task UpdateItem(Guid gameId, ItemCreateDto itemDto, string base64Item)
    {
        bool didParse = Guid.TryParse(Context.UserIdentifier, out Guid userId);
        if(!didParse)
        {
            await Clients.Caller.SendAsync("Error", "Invalid User Login");
            return;
        }

        try
        {
            var game = await gameService.GetAsync(gameId, userId);
            if(game == null)
            {
                await Clients.Caller.SendAsync("Error", "Either Game doesn't exist or you are not authorized to access");
                return;
            }

            byte[] imageArray = Convert.FromBase64String(base64Item);
            string imageName = Guid.NewGuid() + DateTime.Now.ToBinary().ToString() + ".jpg";

            var ms = new MemoryStream(imageArray, false);
            string imageUrl = await blobService.UploadImage("items", imageName, ms);

            Item item = new()
            {
                Name = itemDto.Name,
                Description = itemDto.Description,
                ImageName = imageUrl
            };

            game.Items.Add(item);
            gameService.UpdateAsync(game);
            await gameService.SaveChangesAsync();
        }
        catch(ArgumentNullException ane)
        {
            logger.LogError(ane.Message);
            await Clients.Caller.SendAsync("Error", "One of the information is missing or not valid. Please try again.");
        }
        catch(FormatException fe)
        {
            logger.LogError(fe.Message);
            await Clients.Caller.SendAsync("Error", "The given image is not in the correct format. Please try again with correct image.");
        }
        catch(Exception e)
        {
            logger.LogError(e.Message);
            await Clients.Caller.SendAsync("Error", "An unknown error occured. Please try again later.");
        }
    }
}