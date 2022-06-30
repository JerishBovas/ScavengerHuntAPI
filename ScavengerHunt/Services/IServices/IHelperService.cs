using ScavengerHunt.Models;

namespace ScavengerHunt.Services;

public interface IHelperService
{
    Task<User?> GetCurrentUser(HttpContext context);
    Task<User?> GetUserFromEmail(string email);
}