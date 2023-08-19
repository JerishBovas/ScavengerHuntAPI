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
    public class UsersController : ControllerBase
    {
        private readonly IUserService userRepo;
        private readonly ILogger<UsersController> logger;
        private readonly IHelperService helpService;
        private readonly IBlobService blobService;
        private readonly IMapper mapper;

        public UsersController(IUserService userRepo, ILogger<UsersController> logger, IHelperService helpService, IBlobService blobService, IMapper mapper)
        {
            this.userRepo = userRepo;
            this.logger = logger;
            this.helpService = helpService;
            this.blobService = blobService;
            this.mapper = mapper;
        }

        //GET: /accounts/
        [Authorize, HttpGet]
        public async Task<ActionResult<UserDto>> Get()
        {
            try
            {
                string userId = helpService.GetCurrentUserId(this.User);
                var user = await userRepo.GetAsync(userId);
                if (user == null){ return NotFound(new CustomError("Login Error", 404, new string[] { "The User doesn't exist" }));}

                return mapper.Map<UserDto>(user);
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                return StatusCode(503, new CustomError("Internal Server Error", 503, new string[]{"An internal error occured. Please email to jerishbradlyb@gmail.com and we will try to fix it."}));
            }
        }

        [Authorize, HttpPost]
        public async Task<ActionResult<UserDto>> Create([FromBody] string name)
        {
            try
            {
                string userId = helpService.GetCurrentUserId(this.User);
                
                User newUser = new(userId, name);

                await userRepo.CreateAsync(newUser);
                await userRepo.SaveChangesAsync();
                return Ok(mapper.Map<UserDto>(newUser));
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                return StatusCode(503, new CustomError("Internal Server Error", 503, new string[] { "An internal error occured. Please email to jerishbradlyb@gmail.com and we will try to fix it." }));
            }
        }

        //PUT: /accounts/profileimage
        [Authorize, HttpPut("profileImage")]
        public async Task<ActionResult> AddImage()
        {
            try
            {

                string userId = helpService.GetCurrentUserId(this.User);
                var user = await userRepo.GetAsync(userId);
                if (user == null)
                {
                    return NotFound(new CustomError("Login Error", 404, new string[] { "The User doesn't exist" }));
                }

                var form = await Request.ReadFormAsync();
                var imageFile = form.Files.GetFile("image");

                if(imageFile == null) return BadRequest(new CustomError("Invalid Image", 400, new string[]{"Please upload a valid image"}));

                string name = Guid.NewGuid().ToString() + DateTime.Now.ToBinary().ToString();
                string url = await blobService.UploadImage("users", name, imageFile.OpenReadStream());
                blobService.DeleteImage("users", user.ProfileImage.Split('/').Last());
                user.ProfileImage = url;
                
                await userRepo.SaveChangesAsync();
                return Ok(mapper.Map<UserDto>(user));
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                return StatusCode(503, new CustomError("Internal Server Error", 503, new string[]{"An internal error occured. Please email to jerishbradlyb@gmail.com and we will try to fix it."}));
            }
        }

        //PUT: /accounts/changename
        [Authorize, HttpPut("name")]
        public async Task<ActionResult> ChangeName([FromBody] string name)
        {
            try
            {
                string userId = helpService.GetCurrentUserId(this.User);
                var user = await userRepo.GetAsync(userId);
                if (user == null)
                {
                    return NotFound(new CustomError("Login Error", 404, new string[] { "The User doesn't exist" }));
                }
                
                user.Name = name;

                userRepo.UpdateAsync(user);
                await userRepo.SaveChangesAsync();

                return Ok(mapper.Map<UserDto>(user));
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                return StatusCode(503, new CustomError("Internal Server Error", 503, new string[]{"An internal error occured. Please email to jerishbradlyb@gmail.com and we will try to fix it."}));
            }
        }
    }
}
