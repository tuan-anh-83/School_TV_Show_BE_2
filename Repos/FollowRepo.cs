using BOs.Models;
using DAOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repos
{
    public class FollowRepo : IFollowRepo
    {
        public Task AddFollowAsync(Follow follow)
        {
            return FollowDAO.Instance.AddFollowAsync(follow);
        }

        public async Task<List<Follow>> GetAllFollowsAsync()
        {
            return await FollowDAO.Instance.GetAllFollowsAsync();   
        }

        public async Task<Follow> GetFollowAsync(int accountId, int schoolChannelId)
        {
            return await FollowDAO.Instance.GetFollowAsync(accountId, schoolChannelId);
        }

        public async Task<int> GetFollowCountAsync(int schoolChannelId)
        {
            return await FollowDAO.Instance.GetFollowCountAsync(schoolChannelId);
        }

        public async Task<bool> IsFollowingAsync(int accountId, int schoolChannelId)
        {
            return await FollowDAO.Instance.IsFollowingAsync(accountId, schoolChannelId);   
        }

        public Task UpdateFollowStatusAsync(int accountId, int schoolChannelId, string status)
        {
            return FollowDAO.Instance.UpdateFollowStatusAsync(accountId, schoolChannelId, status);    
        }
    }
}
