using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ScavengerHunt.DTOs;
using ScavengerHunt.Models;
using ScavengerHunt.Services;

namespace ScavengerHunt.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserService userRepo;
        private readonly ILogger<UsersController> logger;
        private readonly IHelperService helpService;
        private readonly IBlobService blobService;

        public UsersController(IUserService userRepo, ILogger<UsersController> logger, IHelperService helpService, IBlobService blobService)
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

            try
            {
                var users = await userRepo.GetAllAsync();

                foreach (var user in users)
                {
                    UserDto userDto = new()
                    {
                        Id = user.Id,
                        Name = user.Name,
                        ProfileImage = user.ProfileImage,
                        Score = user.Score,
                        Games = user.Games,
                        Teams = user.Teams,
                        LastUpdated = user.LastUpdated,
                    };
                    usersDto.Add(userDto);
                }
            }
            catch(Exception ex)
            {
                logger.LogError("Database error: {message}", ex.Message);
            }
            return usersDto;
        }

        //GET: /accounts/
        [Authorize, HttpGet]
        public async Task<ActionResult<UserDto>> Get()
        {
            var user = await helpService.GetCurrentUser(HttpContext);
            if (user is null) { return NotFound("User not found"); }

            UserDto userdt = new()
            {
                Id = user.Id,
                Name = user.Name,
                ProfileImage = user.ProfileImage,
                Score = user.Score,
                Games = user.Games,
                Teams = user.Teams,
                LastUpdated = user.LastUpdated,
            };

            return userdt;
        }

        //PUT: /accounts/profileimage
        [Authorize, HttpPut("profileImage")]
        public async Task<ActionResult> AddImage([FromForm] ImageForm file)
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
                string url = await blobService.SaveImage(file.ImageFile);
                blobService.DeleteImage(user.ProfileImage);
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
    }
}
