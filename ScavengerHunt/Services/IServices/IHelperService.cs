using System.Security.Claims;
using ScavengerHunt.Models;

namespace ScavengerHunt.Services;

public interface IHelperService
{
    string GetCurrentUserId(ClaimsPrincipal User);
}