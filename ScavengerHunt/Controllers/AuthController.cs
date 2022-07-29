using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ScavengerHunt.DTOs;
using ScavengerHunt.Models;
using ScavengerHunt.Services;
using System.Security.Cryptography;
using System.Security.Claims;

namespace ScavengerHunt.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IUserService userRepo;
    private readonly ITokenService tokenService;
    private readonly ILogger<AuthController> logger;
    private readonly IHelperService helpService;
    private readonly IBlobService blobService;

    public AuthController(ITokenService tokenService, IUserService user, ILogger<AuthController> logger, IHelperService help, IBlobService blob)
    {
        this.tokenService = tokenService;
        userRepo = user;
        this.logger = logger;
        helpService = help;
        blobService = blob;
    }

    //POST: /auth/register
    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<ActionResult> Register([FromBody] RegisterDto request)
    {
        if (await helpService.GetUserFromEmail(request.Email) != null)
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
            PasswordHash = passwordHash,
            PasswordSalt = salt
        };

        List<Claim> claims = new()
        {
            new Claim(ClaimTypes.Name, user.Name),
            new Claim(ClaimTypes.NameIdentifier, user.id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
        };

        var accessToken = tokenService.GenerateAccessToken(claims);
        var refreshToken = tokenService.GenerateRefreshToken();

        user.RefToken = refreshToken;
        user.RefTokenExpiry = DateTime.Now.AddDays(7);

        try
        {
            await userRepo.CreateAsync(user);
            await userRepo.SaveChangesAsync();
        }
        catch(Exception e)
        {
            return StatusCode(502,new CustomError("Bad Gateway Error", 502, new string[]{e.Message}));
        }

        return Ok(new AuthResponseDto
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken
        });
    }

    //POST: /auth/login
    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<ActionResult<AuthResponseDto>> Login([FromBody] LoginDto request)
    {
        var user = await helpService.GetUserFromEmail(request.Email);

        if (user == null){ return NotFound(new CustomError("Login Error", 404, new string[]{"Current user doesn't exist"}));}

        if(!VerifyPassword(request.Password, user.PasswordHash, user.PasswordSalt))
        {
            return BadRequest(
                new CustomError("Login Error", 400, new string[]{"Password is incorrect"})
            );
        }

        List<Claim> claims = new()
        {
            new Claim(ClaimTypes.Name, user.Name),
            new Claim(ClaimTypes.NameIdentifier, user.id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
        };

        var accessToken = tokenService.GenerateAccessToken(claims);
        var refreshToken = tokenService.GenerateRefreshToken();

        user.RefToken = refreshToken;
        user.RefTokenExpiry = DateTime.Now.AddDays(7);
        try
        {
            await userRepo.SaveChangesAsync();
        }
        catch(Exception e)
        {
            return StatusCode(502,new CustomError("Bad Gateway Error", 502, new string[]{e.Message}));
        }

        return Ok(new AuthResponseDto
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken
        });
    }

    //POST /auth/refreshtoken
    [HttpPost]
    [Route("refreshtoken")]
    public async Task<IActionResult> Refresh(AuthResponseDto tokenApiModel)
    {
        string accessToken = tokenApiModel.AccessToken;
        string refreshToken = tokenApiModel.RefreshToken;

        User? user = null;
        var principal = tokenService.GetPrincipalFromExpiredToken(accessToken);
        var id = principal.Claims.SingleOrDefault(u => u.Type == ClaimTypes.NameIdentifier)?.Value;
        bool didParsed = Guid.TryParse(id, out Guid result);
        if(didParsed)
        {
            user = await userRepo.GetAsync(result);
        }
        if (user is null || user.RefToken != refreshToken || user.RefTokenExpiry <= DateTime.Now)
            return BadRequest(
                new CustomError("Token Error", 400, new string[]{"Invalid client request"})
            );
        
        var newAccessToken = tokenService.GenerateAccessToken(principal.Claims);
        var newRefreshToken = tokenService.GenerateRefreshToken();
        user.RefToken = newRefreshToken;
        await userRepo.SaveChangesAsync();
        return Ok(new AuthResponseDto()
        {
            AccessToken = newAccessToken,
            RefreshToken = newRefreshToken
        });
    }

    //POST /auth/revoketoken
    [HttpPost, Authorize]
    [Route("revoketoken")]
    public async Task<IActionResult> Revoke()
    {
        var user = await helpService.GetCurrentUser(HttpContext);
        if (user == null) return NotFound(
            new CustomError("Login Error", 404, new string[]{"The User doesn't exist"})
        );
        user.RefToken = null;
        await userRepo.SaveChangesAsync();
        return NoContent();
    }

    //PUT: /auth/resetpassword
    [Authorize]
    [HttpPut("resetpassword")]
    public async Task<ActionResult> ResetPassword(LoginDto res)
    {
        var user = await helpService.GetCurrentUser(HttpContext);
        if (user is null)
        {
            return NotFound(new CustomError("Login Error", 404, new string[]{"The User doesn't exist"}));
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
            logger.LogInformation("Possible Database Error", e);
            return StatusCode(502,new CustomError("Bad Gateway Error", 502, new string[]{e.Message, "Visit https://sh.jerishbovas.com/help"}));
        }

        return Ok();
    }

    [Authorize]
    [HttpPut("AddImage")]
    public async Task<ActionResult> AddImage([FromForm] FileModel file)
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
            string url = await blobService.SaveImage("profile", file.ImageFile, user.id.ToString());
            blobService.DeleteImage("profile", user.ProfileImage);
            user.ProfileImage = url;
            userRepo.UpdateAsync(user);
            await userRepo.SaveChangesAsync();
            return Created(url, new {ImagePath = user.ProfileImage});
        }
        catch (Exception e)
        {
            logger.LogInformation("Possible Storage Error", e);
            return StatusCode(502,new CustomError("Bad Gateway Error", 502, new string[]{e.Message, "Visit https://sh.jerishbovas.com/help"}));
        }
    }

    //PUT: /auth/changename
    [Authorize]
    [HttpPut("changename")]
    public async Task<ActionResult> ChangeName(RegisterDto res)
    {
        var user = await helpService.GetCurrentUser(HttpContext);
        if (user == null)
        {
            return NotFound(new CustomError("Login Error", 404, new string[]{"The User doesn't exist"}));
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
            logger.LogInformation("Possible Database Error", e);
            return StatusCode(502,new CustomError("Bad Gateway Error", 502, new string[]{e.Message, "Visit https://sh.jerishbovas.com/help"}));
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
