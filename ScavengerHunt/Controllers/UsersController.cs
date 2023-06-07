﻿using AutoMapper;
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

        //GET: /accounts/all
        [HttpGet("all"), Authorize]
        public async Task<ActionResult<List<UserDto>>> GetAll()
        {
            try
            {
                var users = await userRepo.GetAllAsync();
                return mapper.Map<List<UserDto>>(users);
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                return StatusCode(503, new CustomError("Internal Server Error", 503, new string[]{"An internal error occured. Please email to jerishbradlyb@gmail.com and we will try to fix it."}));
            }
        }

        //GET: /accounts/
        [Authorize, HttpGet]
        public async Task<ActionResult<UserDto>> Get()
        {
            try
            {
                var user = await helpService.GetCurrentUser(HttpContext);
                if (user is null){return NotFound(new CustomError("Bad Request", 404, new string[]{"User does not exist"}));}

                return mapper.Map<UserDto>(user);
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                return StatusCode(503, new CustomError("Internal Server Error", 503, new string[]{"An internal error occured. Please email to jerishbradlyb@gmail.com and we will try to fix it."}));
            }
        }

        //PUT: /accounts/profileimage
        [Authorize, HttpPut("profileImage")]
        public async Task<ActionResult> AddImage()
        {
            try
            {

                var user = await helpService.GetCurrentUser(HttpContext);
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
                var user = await helpService.GetCurrentUser(HttpContext);
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
