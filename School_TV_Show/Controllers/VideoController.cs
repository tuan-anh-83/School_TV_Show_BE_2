using BOs.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using School_TV_Show.DTO;
using Services;
using Services.CloudFlareService;

namespace School_TV_Show.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class VideoController : ControllerBase
    {
        private readonly IVideoService _videoService;
        private readonly ILogger<VideoController> _logger;

        public VideoController(
            IVideoService videoService,
            ILogger<VideoController> logger)
        {
            _videoService = videoService;
            _logger = logger;
        }

        [HttpGet("all")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllVideos()
        {
            var videos = await _videoService.GetAllVideosAsync();
            return Ok(videos);
        }

        [HttpGet("active")]
        [Authorize(Roles = "User,SchoolOwner")]
        public async Task<IActionResult> GetAllActiveVideos()
        {
            var videos = await _videoService.GetAllVideosAsync();
            var filteredVideos = videos.Where(video => video.Status == true);
            return Ok(filteredVideos);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetVideoById(int id)
        {
            var video = await _videoService.GetVideoByIdAsync(id);
            if (video == null)
                return NotFound(new { message = "Video not found" });

            return Ok(video);
        }

        [HttpPost("add")]
        [Authorize(Roles = "SchoolOwner")]
        public async Task<IActionResult> AddVideo([FromBody] CreateVideoHistoryRequest request)
        {
            if (request == null || string.IsNullOrEmpty(request.URL) || string.IsNullOrEmpty(request.Type))
                return BadRequest(new { message = "Invalid video data" });

            var videoHistory = new VideoHistory
            {
                Description = request.Description,
                ProgramID = request.ProgramID,
                Status = true,
                Type = request.Type,
                URL = request.URL,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                StreamAt = DateTime.UtcNow
            };

            var result = await _videoService.AddVideoAsync(videoHistory);
            if (!result)
                return StatusCode(500, "Error creating video");

            return Ok(new { message = "Video added successfully" });
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "SchoolOwner")]
        public async Task<IActionResult> UpdateVideo(int id, [FromBody] UpdateVideoHistoryRequest request)
        {
            var videoHistory = await _videoService.GetVideoByIdAsync(id);
            if (videoHistory == null)
                return NotFound(new { message = "Video history not found" });

            videoHistory.URL = request.URL;
            videoHistory.Type = request.Type;
            videoHistory.Description = request.Description;
            videoHistory.ProgramID = request.ProgramID;
            videoHistory.UpdatedAt = DateTime.UtcNow;

            var result = await _videoService.UpdateVideoAsync(videoHistory);
            if (!result)
                return StatusCode(500, "Error updating video");

            return Ok(new { message = "Video updated successfully" });
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "SchoolOwner,Admin")]
        public async Task<IActionResult> DeleteVideo(int id)
        {
            var result = await _videoService.DeleteVideoAsync(id);
            if (!result)
                return NotFound(new { message = "Video not found" });

            return Ok(new { message = "Video deleted successfully" });
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("dashboard/totalVideos")]
        public async Task<IActionResult> GetTotalVideos()
        {
            var totalVideos = await _videoService.GetTotalVideosAsync();
            return Ok(new { totalVideos });
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("dashboard/totalVideosByStatus")]
        public async Task<IActionResult> GetTotalVideosByStatus([FromQuery] bool status)
        {
            var totalVideos = await _videoService.GetTotalVideosByStatusAsync(status);
            return Ok(new { totalVideos, status = status ? "Active" : "Inactive" });
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("dashboard/totalViewsAndLikes")]
        public async Task<IActionResult> GetTotalViewsAndLikes()
        {
            var (totalViews, totalLikes) = await _videoService.GetTotalViewsAndLikesAsync();
            return Ok(new { totalViews, totalLikes });
        }

        [HttpGet("{id}/playback")]
        public async Task<IActionResult> GetVideoPlaybackUrl(int id)
        {
            var video = await _videoService.GetVideoByIdAsync(id);
            if (video == null || string.IsNullOrEmpty(video.PlaybackUrl))
                return NotFound(new { message = "Playback URL not found." });

            return Ok(new
            {
                video.VideoHistoryID,
                video.PlaybackUrl
            });
        }
    }

}
