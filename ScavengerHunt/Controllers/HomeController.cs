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
        private readonly IUserService userRepo;
        private readonly ILogger<HomeController> logger;
        private readonly IHelperService helpService;
        private readonly IBlobService blobService;

        public HomeController(IUserService user, ILogger<HomeController> logger, IHelperService help, IBlobService blob)
        {
            userRepo = user;
            this.logger = logger;
            helpService = help;
            blobService = blob;
        }

        //GET /home
        [Authorize, HttpGet("")]
        public async Task<ActionResult<UserDto>> GetInfo()
        {
            User? user;
            UserDto userdt;

            user = await helpService.GetCurrentUser(HttpContext);
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

            user = await helpService.GetCurrentUser(HttpContext);
            if (user is null){return NotFound("User does not exist");}

            foreach(var scorelog in user.UserLog.ScoreLog)
            {
                ScoreLogDto newdt = new()
                {
                    DatePlayed = scorelog.DatePlayed,
                    GameName = scorelog.GameName,
                    Score = scorelog.Score,
                };
                scoreloglist.Add(newdt);
            }

            return scoreloglist;
        }

        [Authorize]
        [HttpPut("UploadImage")]
        public async Task<ActionResult> UploadImage([FromForm] FileModel file)
        {
            if(file.ImageFile == null) return BadRequest();

            var user = await helpService.GetCurrentUser(HttpContext);
            if (user == null)
            {
                return NotFound(
                    new CustomError("Login Error", 404, new string[]{"The User doesn't exist"}));
            }

            try
            {
                string date = DateTime.Now.ToBinary().ToString();
                string name = user.Id.ToString() + date;
                string url = await blobService.SaveImage("images", file.ImageFile, name);
                return Created(url, new {ImagePath = url});
            }
            catch (Exception e)
            {
                logger.LogInformation("Possible Storage Error", e);
                return StatusCode(502,new CustomError("Bad Gateway Error", 502, new string[]{e.Message, "Visit https://sh.jerishbovas.com/help"}));
            }
        }
    }
}
