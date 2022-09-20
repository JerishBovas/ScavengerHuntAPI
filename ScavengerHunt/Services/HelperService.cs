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
                if(!didParse){ return null;}

                User? user = await userService.GetAsync(result);
                if(user != null) return user;

                return null;
            }
        }
        return null;
    }

    public async Task<Account?> GetCurrentAccount(HttpContext context)
    {
        if (context.User.Identity is ClaimsIdentity identity)
        {
            if (identity.Claims.SingleOrDefault(u => u.Type == ClaimTypes.NameIdentifier)?.Value is string id)
            {
                bool didParse = Guid.TryParse(id, out Guid result);
                if (!didParse) { return null; }

                Account? account = await accountService.GetAsync(result);
                if (account != null) return account;

                return null;
            }
        }
        return null;
    }

    public async Task<Account?> GetAccountFromEmail(string email)
    {
        List<Account> users = await accountService.GetAllAsync();
        if(users.Count == 0) return null;
        return users.SingleOrDefault(x => x.Email.ToLower() == email.ToLower());
    }
}