using System.Security.Claims;
using ScavengerHunt.Models;

namespace ScavengerHunt.Services;

public class HelperService : IHelperService
{
    private readonly IUserService userService;
    private readonly IAccountService accountService;

    public HelperService(IUserService userRepo, IAccountService accountService)
    {
        this.userService = userRepo;
        this.accountService = accountService;
    }

    public async Task<User?> GetCurrentUser(HttpContext context)
    {
        if (context.User.Identity is ClaimsIdentity identity)
        {
            if (identity.Claims.SingleOrDefault(u => u.Type == ClaimTypes.NameIdentifier)?.Value is string id)
            {
                bool didParse = Guid.TryParse(id, out Guid result);
                if(didParse)
                {
                    return await userService.GetAsync(result);
                }
            }
            else if(identity.Claims.SingleOrDefault(u => u.Type == ClaimTypes.Email)?.Value is string email)
            {
                var account = await accountService.GetAsync(email);
                if(account != null)
                {
                    return await userService.GetAsync(account.UserId);
                }
            }
        }
        return null;
    }

    public Guid? GetCurrentUserId(HttpContext context)
    {
        if (context.User.Identity is ClaimsIdentity identity)
        {
            if (identity.Claims.SingleOrDefault(u => u.Type == ClaimTypes.NameIdentifier)?.Value is string id)
            {
                bool didParse = Guid.TryParse(id, out Guid result);
                if(didParse)
                {
                    return result;
                }
            }
        }
        return null;
    }

    public async Task<Account?> GetCurrentAccount(HttpContext context)
    {
        if (context.User.Identity is ClaimsIdentity identity)
        {
            if (identity.Claims.SingleOrDefault(u => u.Type == ClaimTypes.Email)?.Value is string id)
            {
                return await accountService.GetAsync(id);
            }
        }
        return null;
    }
}