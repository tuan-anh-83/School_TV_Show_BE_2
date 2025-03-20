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
        public async Task<bool> AddVideoAsync(VideoHistory videoHistory)
        {
            var result = await _videoRepo.AddVideoAsync(videoHistory);
            if (!result)
            {
                _logger.LogError("Failed to add video.");
            }
            return result;
        }
    }
}
