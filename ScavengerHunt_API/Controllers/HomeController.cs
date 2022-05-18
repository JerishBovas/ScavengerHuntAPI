using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ScavengerHunt_API.DTOs;
using ScavengerHunt_API.Library;
using ScavengerHunt_API.Models;
using ScavengerHunt_API.Services;
using System.Collections.Generic;
using System.Security.Claims;

namespace ScavengerHunt_API.Controllers
{
    [Route("api/home")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        private readonly IUserRepository userRepo;

        public HomeController(IUserRepository user)
        {
            userRepo = user;
        }

        //GET /home
        [Authorize]
        [HttpGet("")]
        public async Task<ActionResult> GetInfo()
        {
            string email;
            User? user;
            UserDto userdt;

            email = ExtMethods.GetCurrentUser(HttpContext);
            if (email is ""){return BadRequest("User not logged in");}

            user = await userRepo.GetByEmailAsync(email);
            if (user == null){return NotFound("User not found");}

            userdt = new()
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email
            };

            return Content(JsonConvert.SerializeObject(userdt, Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }));
        }

        //GET /home/userlog
        [Authorize]
        [HttpGet("userlog")]
        public async Task<ActionResult> GetUserLog()
        {
            string email;
            UserLog? userlog;
            IEnumerable<Location> locations;
            IEnumerable<Group> groups;
            UserLogDto userlogdt;
            List<ScoreLogDto> scoreloglist = new();

            email = ExtMethods.GetCurrentUser(HttpContext);
            if (email == ""){return BadRequest("User not logged in");}
            if (userRepo.CheckEmailAsync(email) is null){return NotFound("User not found");}

            userlog = await userRepo.GetUserLogAsync(email);
            if (userlog == null) { return NoContent(); }

            locations = await userRepo.GetLocationsAsync(email);
            groups = await userRepo.GetGroupsAsync(email);

            foreach(ScoreLog scorelog in userlog.ScoreLog)
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

            userlogdt = new()
            {
                UserEmail = email,
                UserScore = userlog.UserScore,
                LastUpdated = userlog.LastUpdated,
                ScoreLog = scoreloglist,
                MyLocations = locations.Count(),
                MyGroups = groups.Count(),
            };

            return Content(JsonConvert.SerializeObject(userlogdt, Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }));
        }
    }
}
