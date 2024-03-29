﻿using ScavengerHunt.Models;

namespace ScavengerHunt.Services
{
    public interface IUserService
    {
        Task<User?> GetAsync(string id);
        Task<List<User>> GetAllAsync();
        Task CreateAsync(User entity);
        void UpdateAsync(User entity);
        void DeleteAsync(User entity);
        Task SaveChangesAsync();
    }
}
