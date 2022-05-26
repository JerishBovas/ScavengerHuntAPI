﻿using Microsoft.EntityFrameworkCore;
using ScavengerHunt.Data;
using ScavengerHunt.Models;

namespace ScavengerHunt.Services
{
    public class GroupRepository : IGroupRepository
    {
        private readonly ScavengerHuntContext context;
        private readonly DbSet<Group> dbSet;

        public GroupRepository(ScavengerHuntContext context)
        {
            this.context = context;
            dbSet = this.context.Set<Group>();
        }

        public async Task<List<Group>> GetAllAsync()
        {
            return await dbSet.ToListAsync();
        }

        public async Task<Group?> GetAsync(Guid id)
        {
            Group? group = await dbSet
                .Where(p => p.Id == id)
                .Include(x => x.PastWinners)
                .FirstOrDefaultAsync();
            return group;
        }

        public async Task CreateAsync(Group entity)
        {
            await dbSet.AddAsync(entity);
        }

        public void UpdateAsync(Group entity)
        {
            dbSet.Update(entity);
        }

        public void DeleteAsync(Group entity)
        {
            dbSet.Remove(entity);
        }

        public async Task SaveChangesAsync()
        {
            await context.SaveChangesAsync();
        }
    }
}