using BOs.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public interface IFollowService
    {
        Task AddFollowAsync(Follow follow);
        Task UpdateFollowStatusAsync(int accountId, int schoolChannelId, string status);
        Task<int> GetFollowCountAsync(int schoolChannelId);
        Task<List<Follow>> GetAllFollowsAsync();
        Task<Follow> GetFollowAsync(int accountId, int schoolChannelId);
        Task<bool> IsFollowingAsync(int accountId, int schoolChannelId);
    }
}
