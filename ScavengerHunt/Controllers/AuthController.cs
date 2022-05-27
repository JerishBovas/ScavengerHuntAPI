using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ScavengerHunt.DTOs;
using static ScavengerHunt.Library.ExtMethods;
using ScavengerHunt.Models;
using ScavengerHunt.Services;
using System.Security.Cryptography;
using System.Security.Claims;

namespace ScavengerHunt.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IRepositoryService<User> userRepo;
        private readonly ITokenService tokenService;
        private readonly ILogger<AuthController> logger;

        public AuthController(ITokenService tokenService, IRepositoryService<User> user, ILogger<AuthController> logger)
        {
            this.tokenService = tokenService;
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
            if (await GetUserFromEmail(request.Email, userRepo) is not null){ return BadRequest("Email already Exist");}

            CreatePassword(request.Password, out string passwordHash, out string salt);

            user = new User(request.Name, request.Email.ToLower(), passwordHash, salt);

            try
            {
                await userRepo.CreateAsync(user);
                await userRepo.SaveChangesAsync();
            }
            catch(Exception e)
            {
                return BadRequest(e.Message);
            }

            return CreatedAtAction(nameof(Register), new { user.Name, user.Email, user.Id});
        }

        //POST: /security/login
        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<ActionResult<string>> Login([FromBody] LoginDto request)
        {
            User? user = await GetUserFromEmail(request.Email, userRepo);

            if (user == null){ return NotFound("User not found");}

            if(!VerifyPassword(request.Password, user.PasswordHash, user.PasswordSalt))
            {
                return NotFound("Password is incorrect");
            }

            List<Claim> claims = new()
            {
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
            };

            var accessToken = tokenService.GenerateAccessToken(claims);
            var refreshToken = tokenService.GenerateRefreshToken();

            user.RefToken = refreshToken;
            user.RefTokenExpiry = DateTime.Now.AddDays(7);
            await userRepo.SaveChangesAsync();

            return Ok(new AuthResponseDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken
            });
        }

        //PUT: /security/resetpassword
        [Authorize]
        [HttpPut("resetpassword")]
        public async Task<ActionResult> ResetPassword(LoginDto res)
        {
            User? user;

            if (!ModelState.IsValid){ return BadRequest(ModelState); }

            user = await GetCurrentUser(HttpContext, userRepo);
            if (user is null)
            {
                return NotFound("User not found");
            }

            CreatePassword(res.Password, out string passwordHash, out string salt);

            user.PasswordHash = passwordHash;
            user.PasswordSalt = salt;

            try
            {
                userRepo.UpdateAsync(user);
                await userRepo.SaveChangesAsync();
            }
            catch (Exception e)
            {
                string Error = "Internal Error Occured.  Please Contact the Developer";
                string Help = "Visit https://sh.jerishbovas.com/help for help";
                var result = new {
                    Error, Help
                };
                logger.LogInformation("Possible Database Error", e);
                return NotFound(result);
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

            user = await GetCurrentUser(HttpContext, userRepo);
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
                return NotFound(e.Message);
            }

            return Ok();
        }

        private static void CreatePassword(string password, out string passwordHash, out string passwordSalt)
        {
            using var hmac = new HMACSHA512();
            byte[] salt = hmac.Key;
            byte[] hash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));

            passwordHash = Convert.ToBase64String(hash);
            passwordSalt = Convert.ToBase64String(salt);
        }

        private static bool VerifyPassword(string password, string hash, string salt)
        {
            byte[] passwordHash = Convert.FromBase64String(hash);
            byte[] passwordSalt = Convert.FromBase64String(salt);

            using var hmac = new HMACSHA512(passwordSalt);
            var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            return computedHash.SequenceEqual(passwordHash);
        }
    }
}
