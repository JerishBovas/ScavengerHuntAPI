using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ScavengerHunt_API.Data;
using ScavengerHunt_API.Models;
using System.Collections.Generic;

namespace ScavengerHunt_API.Services
{
    public class UserRepository : IUserRepository
    {
        private readonly ScavengerHunt_APIContext context;
        private DbSet<User> dbSet;
        public UserRepository(ScavengerHunt_APIContext context)
        {
            this.context = context;
            dbSet = this.context.Set<User>();
        }

        public async Task<List<User>> GetAllAsync()
        {
            return await dbSet.ToListAsync();
        }

        public async Task<User?> GetAsync(int id)
        {
            return await dbSet.FindAsync(id);
        }

        public async Task<string> CheckEmailAsync(string email)
        {
            User? user = await dbSet
                .Where(x => x.Email == email)
                .SingleOrDefaultAsync();
            if(user is null)
            {
                return "";
            }
            return email;
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            return await dbSet
                .Where(s => s.Email == email)
                .FirstOrDefaultAsync();
        }

        public async Task<UserLog?> GetUserLogAsync(string email)
        {
            User? user = await dbSet
                .Where(e => e.Email == email)
                .Include(u => u.UserLog)
                .ThenInclude(ul => ul.ScoreLog)
                .Include(l => l.Locations)
                .Include(g => g.Groups)
                .FirstOrDefaultAsync();
            if(user != null)
            {
                return user.UserLog;
            }
            return null;
        }

        public async Task<IEnumerable<Location>> GetLocationsAsync(string email)
        {
            User? user = await dbSet
                .Where(e => e.Email == email)
                .Include(u => u.Locations)
                .FirstOrDefaultAsync();
            if (user != null)
            {
                return user.Locations.ToList();
            }
            return new List<Location>();
        }

        public async Task<IEnumerable<Group>> GetGroupsAsync(string email)
        {
            User? user = await dbSet
                .Where(e => e.Email == email)
                .Include(u => u.Groups)
                .FirstOrDefaultAsync();
            if (user != null)
            {
                return user.Groups.ToList();
            }
            return new List<Group>();
        }

        public async Task CreateAsync(User entity)
        {
            await dbSet.AddAsync(entity);
        }

        public void UpdateAsync(User entity)
        {
            dbSet.Update(entity);
        }

        public async void DeleteAsync(string email)
        {
            User? entity = await GetByEmailAsync(email);
            if (entity != null)
            {
                dbSet.Remove(entity);
            }
        }

        public async Task SaveChangesAsync()
        {
            await context.SaveChangesAsync();
        }
    }
}
