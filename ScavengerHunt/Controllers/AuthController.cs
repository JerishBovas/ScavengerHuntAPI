using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ScavengerHunt.DTOs;
using ScavengerHunt.Models;
using ScavengerHunt.Services;
using System.Security.Cryptography;
using System.Security.Claims;

namespace ScavengerHunt.Controllers;

[Route("v1/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IUserService userRepo;
    private readonly IAccountService accountRepo;
    private readonly ITokenService tokenService;
    private readonly ILogger<AuthController> logger;
    private readonly IHelperService helpService;

    public AuthController(ITokenService tokenService, IUserService user, ILogger<AuthController> logger, IHelperService help, IAccountService accountRepo)
    {
        this.tokenService = tokenService;
        userRepo = user;
        this.logger = logger;
        helpService = help;
        this.accountRepo = accountRepo;
    }

    // Register user using email and password. Creates new USER and ACCOUNT.
    // Sets the claim and returns token object.
    // POST: /auth/register
    [AllowAnonymous, HttpPost("register")]
    public async Task<ActionResult> Register([FromBody] RegisterDto request)
    {
        try
        {
            if (await accountRepo.GetAsync(request.Email) != null)
            { 
                return Conflict(new CustomError
                (
                    "Register Error", 409, new string[]{"Email provided already Exists"}
                ));
            }

            CreatePassword(request.Password, out string passwordHash, out string salt);

            var user = new User
            {
                Name = request.Name,
                Email = request.Email.ToLower(),
                Score = 0,
                Games = new(),
                Teams = new(),
                LastUpdated = DateTimeOffset.UtcNow
            };
            var account = new Account()
            {
                Email = request.Email.ToLower(),
                PasswordHash = passwordHash,
                PasswordSalt = salt,
                Roles = "user",
                UserId = user.Id
            };

            List<Claim> claims = new()
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, account.Email),
                new Claim(ClaimTypes.Role, account.Roles)
            };

            var accessToken = tokenService.GenerateAccessToken(claims);
            var refreshToken = tokenService.GenerateRefreshToken();

            account.RefToken = refreshToken;
            account.RefTokenExpiry = DateTime.Now.AddDays(7);

            await userRepo.CreateAsync(user);
            await accountRepo.CreateAsync(account);
            await accountRepo.SaveChangesAsync();
            await userRepo.SaveChangesAsync();

            return Ok(new AuthResponseDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken
            });
        }
        catch(Exception e)
        {
            logger.LogError(e.Message);
            return StatusCode(502,new CustomError("Server Error", 502, new string[]{"Something went wrong on our side. Please try again."}));
        }
    }

    // Login user based on given email and password.
    // Sets the claim and returns token object.
    // POST: /auth/login
    [AllowAnonymous, HttpPost("login")]
    public async Task<ActionResult<AuthResponseDto>> Login([FromBody] LoginDto request)
    {
        try
        {
            var account = await accountRepo.GetAsync(request.Email);

            if (account == null){ return NotFound(new CustomError("Login Error", 404, new string[]{"Current user doesn't exist"}));}

            if(!VerifyPassword(request.Password, account.PasswordHash, account.PasswordSalt))
            {
                return BadRequest(
                    new CustomError("Login Error", 400, new string[]{"Password is incorrect"})
                );
            }

            List<Claim> claims = new()
            {
                new Claim(ClaimTypes.NameIdentifier, account.UserId.ToString()),
                new Claim(ClaimTypes.Email, account.Email),
                new Claim(ClaimTypes.Role, account.Roles)
            };

            var accessToken = tokenService.GenerateAccessToken(claims);
            var refreshToken = tokenService.GenerateRefreshToken();

            account.RefToken = refreshToken;
            account.RefTokenExpiry = DateTime.Now.AddDays(7);

            await accountRepo.SaveChangesAsync();

            return Ok(new AuthResponseDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken
            });
        }
        catch(Exception e)
        {
            logger.LogError(e.Message);
            return StatusCode(502,new CustomError("Server Error", 502, new string[]{"Something went wrong on our side. Please try again."}));
        }
    }

    // Refreshes token based on given refresh token.
    // Invalid refreshtoken returns "session invalid".
    // POST /auth/refreshtoken
    [AllowAnonymous, HttpPost("refreshtoken")]
    public async Task<IActionResult> Refresh(AuthResponseDto tokenApiModel)
    {
        try
        {
            string accessToken = tokenApiModel.AccessToken;
            string refreshToken = tokenApiModel.RefreshToken;

            var principal = tokenService.GetPrincipalFromExpiredToken(accessToken);
            var email = principal.Claims.SingleOrDefault(u => u.Type == ClaimTypes.Email)?.Value;
            
            if(email == null)
            { 
                return BadRequest(new CustomError("Session Expired", 400, new string[]{"Session expired. Please login again."}));
            }

            var account = await accountRepo.GetAsync(email);

            if (account is null || account.RefToken != refreshToken || account.RefTokenExpiry <= DateTime.Now)
                return BadRequest(new CustomError("Session Expired", 400, new string[]{"Session expired. Please login again."})
            );
            
            var newAccessToken = tokenService.GenerateAccessToken(principal.Claims);
            var newRefreshToken = tokenService.GenerateRefreshToken();
            account.RefToken = newRefreshToken;
            account.RefTokenExpiry = DateTime.Now.AddDays(7);
            await accountRepo.SaveChangesAsync();
            
            return Ok(new AuthResponseDto()
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken
            });
        }
        catch (Exception e)
        {
            logger.LogError(e.Message);
            return StatusCode(502,new CustomError("Server Error", 502, new string[]{"Something went wrong on our side. Please try again."}));
        }
    }

    // Revokes JWT token.
    // POST /auth/revoketoken
    [HttpPost("revoketoken"), Authorize]
    public async Task<IActionResult> Revoke()
    {
        try
        {
            var account = await helpService.GetCurrentAccount(HttpContext);
            if (account == null) return Ok();
            account.RefToken = null;
            await accountRepo.SaveChangesAsync();
            return Ok();
        }
        catch(Exception e)
        {
            logger.LogError(e.Message);
            return StatusCode(502,new CustomError("Server Error", 502, new string[]{"Something went wrong on our side. Please try again."}));
        }
    }

    // Resets password of the user.
    // Incomplete implementation. Needs attention.
    // PUT: /auth/resetpassword
    [Authorize, HttpPut("resetpassword")]
    public async Task<ActionResult> ResetPassword(LoginDto res)
    {
        var account = await helpService.GetCurrentAccount(HttpContext);
        if (account is null)
        {
            return NotFound(new CustomError("Login Error", 404, new string[]{"The User doesn't exist"}));
        }

        CreatePassword(res.Password, out string passwordHash, out string salt);

        account.PasswordHash = passwordHash;
        account.PasswordSalt = salt;

        try
        {
            accountRepo.UpdateAsync(account);
            await userRepo.SaveChangesAsync();
        }
        catch (Exception e)
        {
            logger.LogInformation("Possible Database Error", e);
            return StatusCode(502,new CustomError("Bad Gateway Error", 502, new string[]{e.Message, "Visit https://sh.jerishbovas.com/help"}));
        }

        return Ok();
    }

    // Creates password from the given user password.
    private static void CreatePassword(string password, out string passwordHash, out string passwordSalt)
    {
        using var hmac = new HMACSHA512();
        byte[] salt = hmac.Key;
        byte[] hash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));

        passwordHash = Convert.ToBase64String(hash);
        passwordSalt = Convert.ToBase64String(salt);
    }

    // Verifies user entered password with system generated encrypted password.
    private static bool VerifyPassword(string password, string hash, string salt)
    {
        byte[] passwordHash = Convert.FromBase64String(hash);
        byte[] passwordSalt = Convert.FromBase64String(salt);

        using var hmac = new HMACSHA512(passwordSalt);
        var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
        return computedHash.SequenceEqual(passwordHash);
    }
}
