using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ScavengerHunt.API.DTOs;
using ScavengerHunt.API.Library;
using ScavengerHunt.API.Models;
using ScavengerHunt.API.Services;
using System.Security.Cryptography;

namespace ScavengerHunt.API.Controllers
{
    [Route("api/security")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUserRepository userRepo;
        private readonly IConfiguration configuration;
        private readonly ILogger<AuthController> logger;

        public AuthController(IConfiguration configuration, IUserRepository user, ILogger<AuthController> logger)
        {
            this.configuration = configuration;
            userRepo = user;
            this.logger = logger;
        }

        //POST: /security/register
        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<ActionResult> Register(RegisterDto request)
        {
            User user;

            if (!ModelState.IsValid){ return BadRequest(ModelState);}
            if (userRepo.GetAsync(request.Email) is not null){ return BadRequest("Email already Exist");}

            CreatePassword(request.Password, out byte[] passwordHash, out byte[] salt);

            user = new()
            {
                Name = request.Name,
                Email = request.Email.ToLower(),
                PasswordHash = passwordHash,
                PasswordSalt = salt,
            };

            try
            {
                await userRepo.CreateAsync(user);
                await userRepo.SaveChangesAsync();
            }
            catch(Exception e)
            {
                return BadRequest(e.Message);
            }

            return Ok();
        }

        //POST: /security/login
        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<ActionResult<string>> Login(LoginDto request)
        {
            User? user = await userRepo.GetAsync(request.Email);

            if (user == null){ return NotFound("Email not found");}

            if(!VerifyPassword(request.Password, user.PasswordHash, user.PasswordSalt))
            {
                return NotFound("Password is incorrect");
            }

            string token = ExtMethods.GenerateToken(user, configuration);

            return Ok(token);
        }

        //PUT: /security/resetpassword
        [Authorize]
        [HttpPut("resetpassword")]
        public async Task<ActionResult> ResetPassword(LoginDto res)
        {
            User? user;

            if (!ModelState.IsValid){ return BadRequest(ModelState); }

            user = await ExtMethods.GetCurrentUser(HttpContext, userRepo);
            if (user is null)
            {
                return NotFound("User not found");
            }

            CreatePassword(res.Password, out byte[] passwordHash, out byte[] salt);

            user.PasswordHash = passwordHash;
            user.PasswordSalt = salt;

            try
            {
                userRepo.UpdateAsync(user);
                await userRepo.SaveChangesAsync();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

            return Ok();
        }

        //PUT: /security/changename
        [Authorize]
        [HttpPut("changename")]
        public async Task<ActionResult> ChangeName(RegisterDto res)
        {
            User? user;

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            user = await ExtMethods.GetCurrentUser(HttpContext, userRepo);
            if (user == null)
            {
                return NotFound("User not found");
            }

            if (VerifyPassword(res.Password, user.PasswordHash, user.PasswordSalt))
            {
                user.Name = res.Name;
            }

            try
            {
                userRepo.UpdateAsync(user);
                await userRepo.SaveChangesAsync();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

            return Ok();
        }

        private static void CreatePassword(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using var hmac = new HMACSHA512();
            passwordSalt = hmac.Key;
            passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
        }

        private static bool VerifyPassword(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using var hmac = new HMACSHA512(passwordSalt);
            var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            return computedHash.SequenceEqual(passwordHash);
        }
    }
}
