using BOs.Models;
using Microsoft.Extensions.Logging;
using Repos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class VideoService :IVideoService
    {
        private readonly IVideoRepo _videoRepo;
        private readonly ILogger<VideoService> _logger;

        public VideoService(IVideoRepo videoRepo, ILogger<VideoService> logger)
        {
            _videoRepo = videoRepo;
            _logger = logger;
        }
        public async Task<List<VideoHistory>> GetAllVideosAsync()
        {
            return await _videoRepo.GetAllVideosAsync();
        }

        public async Task<VideoHistory?> GetVideoByIdAsync(int videoHistoryId)
        {
            return await _videoRepo.GetVideoByIdAsync(videoHistoryId);
        }

        public async Task<bool> AddVideoAsync(VideoHistory videoHistory)
        {
            var result = await _videoRepo.AddVideoAsync(videoHistory);
            if (!result)
            {
                _logger.LogError("Failed to add video.");
            }
            return result;
        }

        public async Task<bool> UpdateVideoAsync(VideoHistory videoHistory)
        {
            var result = await _videoRepo.UpdateVideoAsync(videoHistory);
            if (!result)
            {
                _logger.LogError($"Failed to update video with ID {videoHistory.VideoHistoryID}.");
            }
            return result;
        }

        public async Task<bool> DeleteVideoAsync(int videoHistoryId)
        {
            var result = await _videoRepo.DeleteVideoAsync(videoHistoryId);
            if (!result)
            {
                _logger.LogError($"Failed to delete video with ID {videoHistoryId}.");
            }
            return result;
        }

        //  Tổng số video history
        public async Task<int> GetTotalVideosAsync()
        {
            return await _videoRepo.CountTotalVideosAsync();
        }

        //  Tổng số video theo trạng thái (true = Active, false = Inactive)
        public async Task<int> GetTotalVideosByStatusAsync(bool status)
        {
            return await _videoRepo.CountByStatusAsync(status);
        }

        // Tổng lượt xem và lượt thích trên tất cả video
        public async Task<(int totalViews, int totalLikes)> GetTotalViewsAndLikesAsync()
        {
            return await _videoRepo.GetTotalViewsAndLikesAsync();
        }

        //  Tổng số video trong khoảng thời gian
        public async Task<int> GetVideosByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _videoRepo.CountByDateRangeAsync(startDate, endDate);
        }
    }
}
