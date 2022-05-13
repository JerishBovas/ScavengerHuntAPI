using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using ScavengerHunt_API.DTOs;
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
        private readonly IGenericRepository<User> userRepo;
        private readonly IConfiguration configuration;

        public AuthController(IConfiguration configuration, IGenericRepository<User> user)
        {
            this.configuration = configuration;
            userRepo = user;
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<ActionResult> Register(RegisterDto request)
        {
            User user = new();

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            CreatePassword(request.Password, out byte[] passwordHash, out byte[] salt);

            user.Name = request.Name;
            user.Email = request.Email;
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
                return BadRequest(e);
            }

            return Ok();
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<ActionResult<string>> Login(LoginDto request)
        {
            List<User> users = await userRepo.GetAllAsync();
            User? user = users.SingleOrDefault(x => x.Email == request.Email);

            if(user == null)
            {
                return NotFound("Email not found");
            }

            if(!VerifyPassword(request.Password, user.PasswordHash, user.PasswordSalt))
            {
                return NotFound("Password is incorrect");
            }

            string token = CreateToken(user);

            return Ok(token);
        }

        private string CreateToken(User user)
        {
            List<Claim> claims = new()
            {
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(ClaimTypes.Email, user.Email),
            };

            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(
                configuration.GetSection("AppSettings:Token").Value));

            var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: cred);

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            return jwt;
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
