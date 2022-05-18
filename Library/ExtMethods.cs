﻿using Microsoft.IdentityModel.Tokens;
using ScavengerHunt_API.Models;
using ScavengerHunt_API.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace ScavengerHunt_API.Library
{
    public static class ExtMethods
    {
        public static string GetCurrentUser(HttpContext context)
        {
            if (context.User.Identity is ClaimsIdentity identity)
            {
                if (identity.Claims.SingleOrDefault(u => u.Type == ClaimTypes.Email)?.Value is string email)
                {
                    return email;
                }
            }
            return "";
        }
        public static string GenerateToken(User user, IConfiguration configuration)
        {
            List<Claim> claims = new()
            {
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(ClaimTypes.Email, user.Email),
            };

            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(
                configuration.GetSection("Jwt:Key").Value));

            var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                configuration["Jwt:Issuer"],
                configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: cred);

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            return jwt;
        }
    }
}
