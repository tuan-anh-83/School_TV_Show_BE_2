using BOs.Models;
using Repos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class SchoolChannelFollowService : ISchoolChannelFollowService
    {
        private readonly ISchoolChannelFollowRepo _followRepo;
        public SchoolChannelFollowService(ISchoolChannelFollowRepo followRepo)
        {
            _followRepo = followRepo;
        }
        public async Task AddFollowAsync(SchoolChannelFollow follow)
        {
            await _followRepo.AddFollowAsync(follow);
        }

        public async Task UpdateFollowStatusAsync(int accountId, int schoolChannelId, string status)
        {
            await _followRepo.UpdateFollowStatusAsync(accountId, schoolChannelId, status);
        }

        public async Task<int> GetFollowCountAsync(int schoolChannelId)
        {
            return await _followRepo.GetFollowCountAsync(schoolChannelId);
        }

        public async Task<List<SchoolChannelFollow>> GetAllFollowsAsync()
        {
            return await _followRepo.GetAllFollowsAsync();
        }

        public async Task<SchoolChannelFollow> GetFollowAsync(int accountId, int schoolChannelId)
        {
            return await _followRepo.GetFollowAsync(accountId, schoolChannelId);
        }

        public async Task<bool> IsFollowingAsync(int accountId, int schoolChannelId)
        {
            return await _followRepo.IsFollowingAsync(accountId, schoolChannelId);
        }
        public async Task<IEnumerable<SchoolChannel>> GetFollowedSchoolChannelsAsync(int accountId)
        {
            return await _followRepo.GetFollowedSchoolChannelsAsync(accountId);
        }
        public async Task<List<object>> GetAllFollowedSchoolChannelsAsync()
        {
            return await _followRepo.GetAllFollowedSchoolChannelsAsync();
        }
    }
}
