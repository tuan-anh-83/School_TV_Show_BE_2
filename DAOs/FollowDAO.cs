using BOs.Data;
using BOs.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAOs
{
    public class FollowDAO
    {
        private static FollowDAO instance = null;
        private readonly DataContext _context;

        private FollowDAO()
        {
            _context = new DataContext();
        }

        public static FollowDAO Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new FollowDAO();
                }
                return instance;
            }
        }

        public async Task AddFollowAsync(Follow follow)
        {
            var existingFollow = await _context.Follows
                .FirstOrDefaultAsync(f => f.AccountID == follow.AccountID && f.SchoolChannelID == follow.SchoolChannelID);

            if (existingFollow == null)
            {
                _context.Follows.Add(follow);
            }
            else if (existingFollow.Status == "Unfollowed")
            {
                existingFollow.Status = "Followed";
                existingFollow.FollowedAt = System.DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();
        }

        public async Task UpdateFollowStatusAsync(int accountId, int schoolChannelId, string status)
        {
            var follow = await _context.Follows
                .FirstOrDefaultAsync(f => f.AccountID == accountId && f.SchoolChannelID == schoolChannelId);

            if (follow != null)
            {
                follow.Status = status;
                follow.FollowedAt = System.DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }
        }

        public async Task<int> GetFollowCountAsync(int schoolChannelId)
        {
            return await _context.Follows
                .Where(f => f.SchoolChannelID == schoolChannelId && f.Status == "Followed")
                .CountAsync();
        }

        public async Task<List<Follow>> GetAllFollowsAsync()
        {
            return await _context.Follows
                .Include(f => f.Account)
                .Include(f => f.SchoolChannel)
                .ToListAsync();
        }

        public async Task<Follow> GetFollowAsync(int accountId, int schoolChannelId)
        {
            return await _context.Follows
                .FirstOrDefaultAsync(f => f.AccountID == accountId && f.SchoolChannelID == schoolChannelId);
        }

        public async Task<bool> IsFollowingAsync(int accountId, int schoolChannelId)
        {
            return await _context.Follows
                .AnyAsync(f => f.AccountID == accountId && f.SchoolChannelID == schoolChannelId && f.Status == "Followed");
        }
    }
}
