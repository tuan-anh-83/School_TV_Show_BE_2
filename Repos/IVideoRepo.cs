using BOs.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Repos
{
    public interface IVideoRepo
    {
        Task<List<VideoHistory>> GetAllVideosAsync();
        Task<VideoHistory?> GetVideoByIdAsync(int videoHistoryId);
        Task<bool> AddVideoAsync(VideoHistory videoHistory);
        Task<bool> UpdateVideoAsync(VideoHistory videoHistory);
        Task<bool> DeleteVideoAsync(int videoHistoryId);
        Task<int> CountByStatusAsync(bool status);
        Task<(int totalViews, int totalLikes)> GetTotalViewsAndLikesAsync();
        Task<int> CountByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<int> CountTotalVideosAsync();
        Task<VideoHistory?> GetLatestLiveStreamByProgramIdAsync(int programId);
    }
}
