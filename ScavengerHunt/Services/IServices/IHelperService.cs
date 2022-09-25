using ScavengerHunt.Models;

namespace ScavengerHunt.Services;

public interface IHelperService
{
    Task<User?> GetCurrentUser(HttpContext context);
    Guid? GetCurrentUserId(HttpContext context);
    Task<Account?> GetCurrentAccount(HttpContext context);
}