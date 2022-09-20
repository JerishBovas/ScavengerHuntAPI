using ScavengerHunt.Models;

namespace ScavengerHunt.Services;

public interface IHelperService
{
    Task<User?> GetCurrentUser(HttpContext context);
    Task<Account?> GetCurrentAccount(HttpContext context);
    Task<Account?> GetAccountFromEmail(string email);
}