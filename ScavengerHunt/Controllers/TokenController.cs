using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ScavengerHunt.Models;
using ScavengerHunt.Services;
using static ScavengerHunt.Library.ExtMethods;

[Route("api/[controller]")]
[ApiController]
public class TokenController : ControllerBase
{
    private readonly IRepositoryService<User> userRepo;
    private readonly ITokenService _tokenService;
    public TokenController(IRepositoryService<User> userContext, ITokenService tokenService)
    {
        userRepo = userContext ?? throw new ArgumentNullException(nameof(userContext));
        _tokenService = tokenService ?? throw new ArgumentNullException(nameof(tokenService));
    }
    [HttpPost]
    [Route("refresh")]
    public async Task<IActionResult> Refresh(AuthResponseDto tokenApiModel)
    {
        if (!ModelState.IsValid){ return BadRequest(ModelState); }

        string accessToken = tokenApiModel.AccessToken;
        string refreshToken = tokenApiModel.RefreshToken;

        User? user = null;
        var principal = _tokenService.GetPrincipalFromExpiredToken(accessToken);
        var id = principal.Claims.SingleOrDefault(u => u.Type == ClaimTypes.NameIdentifier)?.Value;
        bool didParsed = Guid.TryParse(id, out Guid result);
        if(didParsed)
        {
            user = await userRepo.GetAsync(result);
        }
        if (user is null || user.RefToken != refreshToken || user.RefTokenExpiry <= DateTime.Now)
            return BadRequest("Invalid client request");
        
        var newAccessToken = _tokenService.GenerateAccessToken(principal.Claims);
        var newRefreshToken = _tokenService.GenerateRefreshToken();
        user.RefToken = newRefreshToken;
        await userRepo.SaveChangesAsync();
        return Ok(new AuthResponseDto()
        {
            AccessToken = newAccessToken,
            RefreshToken = newRefreshToken
        });
    }

    [HttpPost, Authorize]
    [Route("revoke")]
    public async Task<IActionResult> Revoke()
    {
        var user = await GetCurrentUser(HttpContext, userRepo);
        if (user == null) return BadRequest();
        user.RefToken = null;
        await userRepo.SaveChangesAsync();
        return NoContent();
    }
}