using BOs.Models;
using DAOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repos
{
    public class SchoolChannelFollowRepo : ISchoolChannelFollowRepo
    {
        public Task AddFollowAsync(SchoolChannelFollow follow)
        {
            return SchoolChannelFollowDAO.Instance.AddFollowAsync(follow);
        }

        public async Task<List<SchoolChannelFollow>> GetAllFollowsAsync()
        {
            return await SchoolChannelFollowDAO.Instance.GetAllFollowsAsync();   
        }

        public async Task<SchoolChannelFollow> GetFollowAsync(int accountId, int schoolChannelId)
        {
            return await SchoolChannelFollowDAO.Instance.GetFollowAsync(accountId, schoolChannelId);
        }

        public async Task<int> GetFollowCountAsync(int schoolChannelId)
        {
            return await SchoolChannelFollowDAO.Instance.GetFollowCountAsync(schoolChannelId);
        }

        public async Task<bool> IsFollowingAsync(int accountId, int schoolChannelId)
        {
            return await SchoolChannelFollowDAO.Instance.IsFollowingAsync(accountId, schoolChannelId);   
        }

        public Task UpdateFollowStatusAsync(int accountId, int schoolChannelId, string status)
        {
            return SchoolChannelFollowDAO.Instance.UpdateFollowStatusAsync(accountId, schoolChannelId, status);    
        }

        public Task<IEnumerable<SchoolChannel>> GetFollowedSchoolChannelsAsync(int accountId)
        {
            return SchoolChannelFollowDAO.Instance.GetFollowedSchoolChannelsAsync(accountId);
        }

        public Task<List<object>> GetAllFollowedSchoolChannelsAsync()
        {
            return SchoolChannelFollowDAO.Instance.GetAllFollowedSchoolChannelsAsync();
        }
    }
}
