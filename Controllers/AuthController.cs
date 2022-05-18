using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using ScavengerHunt_API.DTOs;
using ScavengerHunt_API.Library;
using ScavengerHunt_API.Models;
using ScavengerHunt_API.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;

namespace ScavengerHunt_API.Controllers
{
    [Route("api/security")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUserRepository userRepo;
        private readonly IConfiguration configuration;

        public AuthController(IConfiguration configuration, IUserRepository user)
        {
            this.configuration = configuration;
            userRepo = user;
        }

        //POST: /security/register
        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<ActionResult> Register(RegisterDto request)
        {
            User user = new();

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (userRepo.CheckEmailAsync(request.Email) is not null)
            {
                ModelState.AddModelError("Email", "Email already Exist");
                return BadRequest(ModelState);
            }

            CreatePassword(request.Password, out byte[] passwordHash, out byte[] salt);

            user.Name = request.Name;
            user.Email = request.Email.ToLower();
            user.PasswordHash = passwordHash;
            user.PasswordSalt = salt;
            user.UserLog = new UserLog();

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
            User? user = await userRepo.GetByEmailAsync(request.Email);

            if (user == null)
            {
                return NotFound("Email not found");
            }

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
            string currentUser = ExtMethods.GetCurrentUser(HttpContext);
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (currentUser == "")
            {
                ModelState.AddModelError("Authentication", "User is not authenticated");
                return BadRequest(ModelState);
            }

            User? user = await userRepo.GetByEmailAsync(currentUser);
            if(user == null)
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
            string currentUser = ExtMethods.GetCurrentUser(HttpContext);
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (currentUser == "")
            {
                ModelState.AddModelError("Authentication", "User is not authenticated");
                return BadRequest(ModelState);
            }
            User? user = await userRepo.GetByEmailAsync(currentUser);
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

        private void CreatePassword(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

        private bool VerifyPassword(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512(passwordSalt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                return computedHash.SequenceEqual(passwordHash);
            }
        }
    }
}
