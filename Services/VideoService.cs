using BOs.Models;
using Microsoft.AspNetCore.Http;
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
        private readonly ICloudflareUploadService _cloudflareUploadService;

        public VideoService(IVideoRepo videoRepo, ILogger<VideoService> logger, ICloudflareUploadService cloudflareUploadService)
        {
            _videoRepo = videoRepo;
            _logger = logger;
            _cloudflareUploadService = cloudflareUploadService;
        }
        public async Task<bool> AddVideoWithCloudflareAsync(IFormFile videoFile, VideoHistory videoHistory)
        {
            try
            {
                var (streamId, playbackUrl, mp4Url) = await _cloudflareUploadService.UploadVideoAsync(videoFile);
                videoHistory.CloudflareStreamId = streamId;
                videoHistory.PlaybackUrl = playbackUrl;
                videoHistory.MP4Url = mp4Url;
                videoHistory.URL = playbackUrl;
                videoHistory.CreatedAt = DateTime.UtcNow;
                videoHistory.UpdatedAt = DateTime.UtcNow;

                return await _videoRepo.AddVideoAsync(videoHistory);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading video to Cloudflare.");
                return false;
            }
        }
        public async Task<List<VideoHistory>> GetAllVideoHistoriesAsync()
        {
            return await _videoRepo.GetAllVideoHistoriesAsync();
        }
        public async Task<List<VideoHistory>> GetAllVideosAsync()
        {
            return await _videoRepo.GetAllVideosAsync();
        }

        public async Task<VideoHistory?> GetVideoByIdAsync(int videoHistoryId)
        {
            return await _videoRepo.GetVideoByIdAsync(videoHistoryId);
        }

        public async Task<VideoHistory?> GetLatestLiveStreamByProgramIdAsync(int programId)
        {
            return await _videoRepo.GetLatestLiveStreamByProgramIdAsync(programId);
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

        public async Task<int> GetTotalVideosAsync()
        {
            return await _videoRepo.CountTotalVideosAsync();
        }

        public async Task<int> GetTotalVideosByStatusAsync(bool status)
        {
            return await _videoRepo.CountByStatusAsync(status);
        }

        public async Task<(int totalViews, int totalLikes)> GetTotalViewsAndLikesAsync()
        {
            return await _videoRepo.GetTotalViewsAndLikesAsync();
        }

        public async Task<int> GetVideosByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _videoRepo.CountByDateRangeAsync(startDate, endDate);
        }
        public async Task<List<VideoHistory>> GetVideosByDateAsync(DateTime date)
        {
            return await _videoRepo.GetVideosByDateAsync(date);
        }
    }
}
