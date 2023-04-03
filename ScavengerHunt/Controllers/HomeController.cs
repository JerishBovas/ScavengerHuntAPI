using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ScavengerHunt.DTOs;
using ScavengerHunt.Models;
using ScavengerHunt.Services;

namespace ScavengerHunt.Controllers
{
    [Route("v1/[controller]")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        private readonly IUserService userRepo;
        private readonly IGameService gameService;
        private readonly ILogger<HomeController> logger;
        private readonly IHostEnvironment _hostingEnvironment;
        private readonly IConfiguration configuration;
        private readonly IMapper mapper;
        private static List<User> leaderboard = new();
        private static DateTime? leaderboardExpiry = null;
        private static List<Game> popularGames = new();
        private static DateTime? popularGamesExpiry = null;

        public HomeController(IUserService user, ILogger<HomeController> logger, IGameService gameService, IHostEnvironment hostingEnvironment, IConfiguration configuration, IMapper mapper)
        {
            userRepo = user;
            this.logger = logger;
            this.gameService = gameService;
            _hostingEnvironment = hostingEnvironment;
            this.configuration = configuration;
            this.mapper = mapper;
        }

        //Get /leaderboard
        [AllowAnonymous, HttpGet("leaderboard")]
        public async Task<ActionResult> GetLeaderBoard()
        {
            try
            {
                if(leaderboardExpiry is null || leaderboardExpiry < DateTime.Now)
            {
                await updateLeaderboard();
                leaderboardExpiry = DateTime.Now.AddMinutes(60);
            }

            return Ok(mapper.Map<List<UserDto>>(leaderboard));
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                return StatusCode(503, new CustomError("Internal Server Error", 503, new string[]{"An internal error occured. Please email to jerishbradlyb@gmail.com and we will try to fix it."}));
            }
        }
    
        //Get /populargames
        [AllowAnonymous, HttpGet("PopularGames")]
        public async Task<ActionResult> GetPopularGames()
        {
            try
            {
                if(popularGamesExpiry is null || popularGamesExpiry < DateTime.Now)
                {
                    await updatePopularGames();
                    popularGamesExpiry = DateTime.Now.AddMinutes(60);
                }
                popularGames.RemoveAll(g => g.IsPrivate == true);

                return Ok(mapper.Map<List<GameDto>>(popularGames));
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                return StatusCode(503, new CustomError("Internal Server Error", 503, new string[]{"An internal error occured. Please email to jerishbradlyb@gmail.com and we will try to fix it."}));
            }
        }

        private async Task updateLeaderboard()
        {
            List<User> users = await userRepo.GetAllAsync();
            users.Sort(delegate(User x, User y) {
                return -1 * (x.Score.CompareTo(y.Score));
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
