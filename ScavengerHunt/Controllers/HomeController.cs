using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ScavengerHunt.DTOs;
using ScavengerHunt.Models;
using ScavengerHunt.Services;

namespace ScavengerHunt.Controllers
{
    [Route("api/home")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        private readonly IRepositoryService<User> userRepo;
        private readonly ILogger<HomeController> logger;
        private readonly IHelperService helpMethod;

        public HomeController(IRepositoryService<User> user, ILogger<HomeController> logger, IHelperService help)
        {
            userRepo = user;
            this.logger = logger;
            helpMethod = help;
        }

        //GET /home
        [Authorize]
        [HttpGet("")]
        public async Task<ActionResult<UserDto>> GetInfo()
        {
            User? user;
            UserDto userdt;

            user = await helpMethod.GetCurrentUser(HttpContext);
            if (user is null){return NotFound("User does not exist");}

            userdt = new()
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                UserLog = new UserLogDto()
                {
                    UserScore = user.UserLog.UserScore,
                    LastUpdated = user.UserLog.LastUpdated,
                }
            };

            return userdt;
        }

        //GET /home/userlog
        [Authorize]
        [HttpGet("scores")]
        public async Task<ActionResult<List<ScoreLogDto>>> GetScoreLog()
        {
            User? user;
            List<ScoreLogDto> scoreloglist = new();

            user = await helpMethod.GetCurrentUser(HttpContext);
            if (user is null){return NotFound("User does not exist");}

            foreach(var scorelog in user.UserLog.ScoreLog)
            {
                ScoreLogDto newdt = new()
                {
                    DatePlayed = scorelog.DatePlayed,
                    LocationName = scorelog.LocationName,
                    Score = scorelog.Score,
                };
                scoreloglist.Add(newdt);
            }

            return scoreloglist;
        }
    }
}
