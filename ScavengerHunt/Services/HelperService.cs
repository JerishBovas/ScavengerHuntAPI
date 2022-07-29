using System.Security.Claims;
using ScavengerHunt.Models;

namespace ScavengerHunt.Services;

public class HelperService : IHelperService
{
    private readonly IUserService userRepo;

    public HelperService(IUserService userRepo)
    {
        this.userRepo = userRepo;
    }
    public async Task<User?> GetCurrentUser(HttpContext context)
    {
        if (context.User.Identity is ClaimsIdentity identity)
        {
            if (identity.Claims.SingleOrDefault(u => u.Type == ClaimTypes.NameIdentifier)?.Value is string id)
            {
                bool didParse = Guid.TryParse(id, out Guid result);
                if(!didParse){ return null;}

                User? user = await userRepo.GetAsync(result);
                if(user != null && user.Email == identity.Claims.SingleOrDefault(u => u.Type == ClaimTypes.Email)?.Value) return user;

                return null;
            }
        }
        return null;
    }
    public async Task<User?> GetUserFromEmail(string email)
    {
        List<User> users = await userRepo.GetAllAsync();
        if(users.Count == 0) return null;
        return users.SingleOrDefault(x => x.Email.ToLower() == email.ToLower());
    }
}