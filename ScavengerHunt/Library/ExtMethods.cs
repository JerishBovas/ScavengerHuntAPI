using Microsoft.IdentityModel.Tokens;
using ScavengerHunt.Models;
using ScavengerHunt.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace ScavengerHunt.Library
{
    public static class ExtMethods
    {
        public async static Task<User?> GetCurrentUser(HttpContext context, IRepositoryService<User> userRepository)
        {
            if (context.User.Identity is ClaimsIdentity identity)
            {
                if (identity.Claims.SingleOrDefault(u => u.Type == ClaimTypes.NameIdentifier)?.Value is string id)
                {
                    bool didParse = Guid.TryParse(id, out Guid result);
                    if(!didParse){ return null;}

                    User? user = await userRepository.GetAsync(result);
                    if(user != null && user.Email == identity.Claims.SingleOrDefault(u => u.Type == ClaimTypes.Email)?.Value) return user;

                    return null;
                }
            }
            return null;
        }
        public async static Task<User?> GetUserFromEmail(string email, IRepositoryService<User> userRepo)
        {
            List<User> users = await userRepo.GetAllAsync();
            
            return users.Where(u => u.Email == email).FirstOrDefault();
        }
    }
}
