using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ScavengerHunt.DTOs;
using ScavengerHunt.Models;
using ScavengerHunt.Services;

namespace ScavengerHunt.Controllers
{
    [Route("")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        private readonly IUserService userRepo;
        private readonly IGameService gameService;
        private readonly ILogger<HomeController> logger;
        private readonly IHelperService helpService;
        private readonly IBlobService blobService;
        private IHostEnvironment _hostingEnvironment;
        private IConfiguration configuration;
        private static List<User> leaderboard = new();
        private static DateTime? leaderboardExpiry = null;
        private static List<Game> popularGames = new();
        private static DateTime? popularGamesExpiry = null;

        public HomeController(IUserService user, ILogger<HomeController> logger, IHelperService help, IBlobService blob, IGameService gameService, IHostEnvironment hostingEnvironment, IConfiguration configuration)
        {
            userRepo = user;
            this.logger = logger;
            helpService = help;
            blobService = blob;
            this.gameService = gameService;
            this._hostingEnvironment = hostingEnvironment;
            this.configuration = configuration;
        }

        //Get /leaderboard
        [AllowAnonymous, HttpGet("api/v1/Home/leaderboard")]
        public async Task<ActionResult> GetLeaderBoard()
        {
            List<UserDto> leaderBoardList = new();

            if(leaderboardExpiry is null || leaderboardExpiry < DateTime.Now)
            {
                await updateLeaderboard();
                leaderboardExpiry = DateTime.Now.AddMinutes(60);
            }
            foreach(var user in leaderboard)
            {
                UserDto newdt = new()
                {
                    Id = user.id,
                    Name = user.Name,
                    Email = user.Email,
                    ProfileImage = user.ProfileImage,
                    UserLog = new(){
                        UserScore = user.UserLog.UserScore,
                        LastUpdated = user.UserLog.LastUpdated
                    }
                };
                leaderBoardList.Add(newdt);
            }

            return Ok(leaderBoardList);
        }
    
        //Get /populargames
        [AllowAnonymous, HttpGet("api/v1/Home/PopularGames")]
        public async Task<ActionResult> GetPopularGames()
        {
            List<GameDto> popularGamesList = new();

            if(popularGamesExpiry is null || popularGamesExpiry < DateTime.Now)
            {
                await updatePopularGames();
                popularGamesExpiry = DateTime.Now.AddMinutes(60);
            }
            foreach(var game in popularGames)
            {
                if(game.IsPrivate) continue;
                GameDto newdt = new()
                {
                    Id = game.id,
                    IsPrivate = game.IsPrivate,
                    Name = game.Name,
                    Description = game.Description,
                    Address = game.Address,
                    Country = game.Country,
                    Coordinate = new(){Latitude = game.Coordinate.Latitude, Longitude = game.Coordinate.Longitude},
                    ImageName = game.ImageName,
                    Difficulty = game.Difficulty,
                    Ratings = game.Ratings.Average(),
                    Tags = game.Tags,
                    CreatedDate = game.CreatedDate,
                    LastUpdated = game.LastUpdated
                };
                popularGamesList.Add(newdt);
            }

            return Ok(popularGamesList);
        }

        //GET: /.well-known/apple-app-site-association
        [AllowAnonymous, HttpGet(".well-known/apple-app-site-association")]
        public async Task<IActionResult> Index()
        {
            return Content(
                await System.IO.File.ReadAllTextAsync(Path.Combine(_hostingEnvironment.ContentRootPath, configuration["AppleSitePath"])),
                "text/plain"
            );
        }

        private async Task updateLeaderboard()
        {
            List<User> users = await userRepo.GetAllAsync();
            users.Sort(delegate(User x, User y) {
                return -1 * (x.UserLog.UserScore.CompareTo(y.UserLog.UserScore));
            });

            leaderboard = users.Take(5).ToList();
        }

        private async Task updatePopularGames()
        {
            List<Game> games = await gameService.GetAllAsync();
            games.Sort(delegate(Game x, Game y) {
                double xVal = x.Ratings.Count == 0 ? 0 : x.Ratings.Average();
                double yVal = y.Ratings.Count == 0 ? 0 : y.Ratings.Average();
                
                return -1 * (xVal.CompareTo(yVal));
            });

            popularGames = games.Take(5).ToList();
        }
    }
}
