using BOs.Models;
using Repos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class FollowService : IFollowService
    {
        private readonly IFollowRepo _followRepo;
        public FollowService(IFollowRepo followRepo)
        {
            _followRepo = followRepo;
        }

        public Task AddFollowAsync(Follow follow)
        {
            return _followRepo.AddFollowAsync(follow);
        }

        public async Task<List<Follow>> GetAllFollowsAsync()
        {
            return await _followRepo.GetAllFollowsAsync();
        }

        public async Task<Follow> GetFollowAsync(int accountId, int schoolChannelId)
        {
            return await _followRepo.GetFollowAsync(accountId, schoolChannelId);    
        }

        public async Task<int> GetFollowCountAsync(int schoolChannelId)
        {
            return await _followRepo.GetFollowCountAsync(schoolChannelId);
        }

        public async Task<bool> IsFollowingAsync(int accountId, int schoolChannelId)
        {
            return await _followRepo.IsFollowingAsync(accountId, schoolChannelId);  
        }

        public Task UpdateFollowStatusAsync(int accountId, int schoolChannelId, string status)
        {
            return _followRepo.UpdateFollowStatusAsync(accountId, schoolChannelId, status);   
        }
    }
}
