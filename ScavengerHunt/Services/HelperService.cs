using System.Security.Claims;
using ScavengerHunt.Models;

namespace ScavengerHunt.Services;

public class HelperService : IHelperService
{
    private readonly IUserService userService;

    public HelperService(IUserService userRepo)
    {
        this.userService = userRepo;
    }

    public string GetCurrentUserId(ClaimsPrincipal User)
    {
        return User.Claims.First(u => u.Type == ClaimTypes.NameIdentifier).Value;
    }
}