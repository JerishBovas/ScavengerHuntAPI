using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ScavengerHunt.API.DTOs;
using ScavengerHunt.API.Library;
using ScavengerHunt.API.Models;
using ScavengerHunt.API.Services;
using System.Collections.Generic;
using System.Security.Claims;

namespace ScavengerHunt.API.Controllers
{
    [Route("api/home")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        private readonly IUserRepository userRepo;
        private readonly IScoreLogRepository scoreLogRepo;
        private readonly ILogger<HomeController> logger;

        public HomeController(IUserRepository user, ILogger<HomeController> logger, IScoreLogRepository scoreLog)
        {
            userRepo = user;
            scoreLogRepo = scoreLog;
            this.logger = logger;
        }

        //GET /home
        [Authorize]
        [HttpGet("")]
        public async Task<ActionResult> GetInfo()
        {
            User? user;
            UserDto userdt;

            user = await ExtMethods.GetCurrentUser(HttpContext, userRepo);
            if (user is null){return NotFound("User does not exist");}

            userdt = new()
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                UserLog = new UserLogDto()
                {
                    Id = user.UserLog.Id,
                    UserEmail = user.Email,
                    UserScore = user.UserLog.UserScore,
                    LastUpdated = user.UserLog.LastUpdated,
                }
            };

            return Content(JsonConvert.SerializeObject(userdt, Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }));
        }

        //GET /home/userlog
        [Authorize]
        [HttpGet("scores")]
        public async Task<ActionResult> GetScoreLog()
        {
            User? user;
            IEnumerable<ScoreLog> scoreLogList;
            List<ScoreLogDto> scoreloglist = new();

            user = await ExtMethods.GetCurrentUser(HttpContext, userRepo);
            if (user is null){return NotFound("User does not exist");}

            scoreLogList = await scoreLogRepo.GetScoreLogsByUser(user);

            foreach(var scorelog in scoreLogList)
            {
                ScoreLogDto newdt = new()
                {
                    Id = scorelog.Id,
                    DatePlayed = scorelog.DatePlayed,
                    LocationName = scorelog.LocationName,
                    Score = scorelog.Score,
                };
                scoreloglist.Add(newdt);
            }

            return Content(JsonConvert.SerializeObject(scoreLogList, Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }));
        }
    }
}
