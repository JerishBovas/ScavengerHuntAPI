using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ScavengerHunt.DTOs;
using ScavengerHunt.Models;
using ScavengerHunt.Services;

namespace ScavengerHunt.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private readonly IUserService userRepo;
        private readonly ILogger<AccountsController> logger;
        private readonly IHelperService helpService;
        private readonly IBlobService blobService;

        public AccountsController(IUserService userRepo, ILogger<AccountsController> logger, IHelperService helpService, IBlobService blobService)
        {
            this.userRepo = userRepo;
            this.logger = logger;
            this.helpService = helpService;
            this.blobService = blobService;
        }

        //GET: /accounts/all
        [HttpGet("all"), Authorize]
        public async Task<ActionResult<List<UserDto>>> GetAll()
        {
            List<UserDto> usersDto = new();

            var users = await userRepo.GetAllAsync();

            if (!users.Any())
            {
                return new List<UserDto>();
            }

            foreach (var user in users)
            {
                UserDto userDto = new()
                {
                    Id = user.id,
                    Name = user.Name,
                    Email = user.Email,
                    ProfileImage = user.ProfileImage,
                    UserLog = new UserLogDto()
                    {
                        UserScore = user.UserLog.UserScore,
                        LastUpdated = user.UserLog.LastUpdated
                    }
                };
                usersDto.Add(userDto);
            }
            return usersDto;
        }

        //GET: /accounts/
        [Authorize, HttpGet]
        public async Task<ActionResult<UserDto>> Get()
        {
            var user = await helpService.GetCurrentUser(HttpContext);
            if (user is null) { return NotFound("User does not exist"); }

            UserDto userdt = new()
            {
                Id = user.id,
                Name = user.Name,
                Email = user.Email,
                ProfileImage = user.ProfileImage,
                UserLog = new UserLogDto()
                {
                    UserScore = user.UserLog.UserScore,
                    LastUpdated = user.UserLog.LastUpdated,
                }
            };

            return userdt;
        }

        //GET: /accounts/scores
        [Authorize, HttpGet("scores")]
        public async Task<ActionResult<List<GameScoreDto>>> GetScores()
        {
            List<GameScoreDto> scores = new();
            var user = await helpService.GetCurrentUser(HttpContext);
            if (user is null) { return NotFound("User does not exist"); }

            foreach(var item in user.UserLog.ScoreLog)
            {
                GameScoreDto scoreLog = new()
                {
                    id = item.id,
                    GameId = item.GameId,
                    GameName = item.GameName,
                    NoOfItems = item.NoOfItems,
                    ItemsFound = item.ItemsFound,
                    Score = item.Score,
                    StartTime = item.StartTime,
                    EndTime = item.EndTime
                };
                scores.Add(scoreLog);
            }
            return scores;
        }

        //PUT: /accounts/profileimage
        [Authorize, HttpPut("profileImage")]
        public async Task<ActionResult> AddImage([FromForm] FileModel file)
        {
            if (file.ImageFile == null) return BadRequest();

            var user = await helpService.GetCurrentUser(HttpContext);
            if (user == null)
            {
                return NotFound(
                    new CustomError("Login Error", 404, new string[] { "The User doesn't exist" }));
            }

            try
            {
                string url = await blobService.SaveImage("profile", file.ImageFile, user.id.ToString());
                blobService.DeleteImage("profile", user.ProfileImage);
                user.ProfileImage = url;
                userRepo.UpdateAsync(user);
                await userRepo.SaveChangesAsync();
                return Created(url, new { ImagePath = user.ProfileImage });
            }
            catch (Exception e)
            {
                logger.LogInformation("Possible Storage Error", e);
                return StatusCode(502, new CustomError("Bad Gateway Error", 502, new string[] { e.Message, "Visit https://sh.jerishbovas.com/help" }));
            }
        }

        //PUT: /accounts/changename
        [Authorize, HttpPut("name")]
        public async Task<ActionResult> ChangeName(RegisterDto res)
        {
            var user = await helpService.GetCurrentUser(HttpContext);
            if (user == null)
            {
                return NotFound(new CustomError("Login Error", 404, new string[] { "The User doesn't exist" }));
            }
            
            user.Name = res.Name;

            try
            {
                userRepo.UpdateAsync(user);
                await userRepo.SaveChangesAsync();
            }
            catch (Exception e)
            {
                logger.LogInformation("Possible Database Error", e);
                return StatusCode(502, new CustomError("Bad Gateway Error", 502, new string[] { e.Message, "Visit https://sh.jerishbovas.com/help" }));
            }

            return Ok();
        }

        //DELETE: /accounts/23
        [Authorize, HttpDelete("{id}")]
        public async Task<ActionResult> Delete(Guid id)
        {
            var user = await helpService.GetCurrentUser(HttpContext);
            if (user is null) { return NotFound("User does not exist"); }
            if(user.id != id) { return BadRequest("User doesn't match"); }

            try
            {
                userRepo.DeleteAsync(id);
                await userRepo.SaveChangesAsync();
            }
            catch (Exception e)
            {
                logger.LogInformation("Possible Database Error", e);
                return StatusCode(502, new CustomError("Bad Gateway Error", 502, new string[] { e.Message, "Visit https://sh.jerishbovas.com/help" }));
            }

            return Ok();
        }
    }
}
