using BOs.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public interface IVideoService
    {
        Task<List<VideoHistory>> GetAllVideosAsync();
        Task<VideoHistory?> GetVideoByIdAsync(int videoHistoryId);
        Task<bool> AddVideoAsync(VideoHistory videoHistory);
        Task<bool> UpdateVideoAsync(VideoHistory videoHistory);
        Task<bool> DeleteVideoAsync(int videoHistoryId);

        Task<int> GetTotalVideosAsync();
        Task<int> GetTotalVideosByStatusAsync(bool status);
        Task<(int totalViews, int totalLikes)> GetTotalViewsAndLikesAsync();
        Task<int> GetVideosByDateRangeAsync(DateTime startDate, DateTime endDate);
    }
}
