using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ScavengerHunt.DTOs;
using ScavengerHunt.Library;
using ScavengerHunt.Models;
using ScavengerHunt.Services;
using System.Collections.Generic;
using System.Security.Claims;

namespace ScavengerHunt.Controllers
{
    [Route("api/home")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        private readonly IUserRepository userRepo;
        private readonly ILogger<HomeController> logger;

        public HomeController(IUserRepository user, ILogger<HomeController> logger)
        {
            userRepo = user;
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
            List<ScoreLogDto> scoreloglist = new();

            user = await ExtMethods.GetCurrentUser(HttpContext, userRepo);
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

            return Content(JsonConvert.SerializeObject(scoreloglist, Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }));
        }
    }
}
