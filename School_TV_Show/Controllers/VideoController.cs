using BOs.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using School_TV_Show.DTO;
using Services;
using Services.CloudFlareService;

namespace School_TV_Show.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class VideoHistoryController : ControllerBase
    {
        private readonly IVideoService _videoService;
        private readonly ILogger<VideoHistoryController> _logger;
        private readonly CloudflareSettings _cloudflareSettings;

        public VideoHistoryController(
            IVideoService videoService,
            ILogger<VideoHistoryController> logger,
            IOptions<CloudflareSettings> cloudflareOptions)
        {
            _videoService = videoService;
            _logger = logger;
            _cloudflareSettings = cloudflareOptions.Value;
        }


        [HttpGet("all")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllVideos()
        {
            var videos = await _videoService.GetAllVideosAsync();
            return Ok(videos);
        }

        [HttpGet("active")]
        [Authorize(Roles = "User,SchoolOwner,Admin")]
        public async Task<IActionResult> GetAllActiveVideos()
        {
            var videos = await _videoService.GetAllVideosAsync();
            var filteredVideos = videos.Where(video => video.Status);
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

        [HttpPost("UploadCloudflare")]
        [Authorize(Roles = "SchoolOwner")]
        public async Task<IActionResult> UploadVideoToCloudflare([FromForm] UploadVideoHistoryRequest request)
        {
            if (request.VideoFile == null || request.VideoFile.Length == 0)
                return BadRequest(new { message = "No video file provided." });

            var videoHistory = new VideoHistory
            {
                ProgramID = request.ProgramID,
                Type = request.Type,
                Description = request.Description,
                Status = true,
                StreamAt = request.StreamAt,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            var result = await _videoService.AddVideoWithCloudflareAsync(request.VideoFile, videoHistory);

            if (!result)
                return StatusCode(500, new { message = "Failed to upload video to Cloudflare." });

            return Ok(new
            {
                message = "Video uploaded successfully.",
                data = new
                {
                    videoId = videoHistory.VideoHistoryID,
                    programId = videoHistory.ProgramID,
                    playbackUrl = videoHistory.PlaybackUrl,
                    mp4Url = videoHistory.MP4Url,
                    iframeUrl = $"https://customer-{_cloudflareSettings.StreamDomain}.cloudflarestream.com/{videoHistory.CloudflareStreamId}/iframe",
                    duration = videoHistory.Duration
                }
            });
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

        [HttpGet("program/{programId}/latest-live")]
        [Authorize(Roles = "SchoolOwner")]
        public async Task<IActionResult> GetLatestLiveByProgramId(int programId)
        {
            var video = await _videoService.GetLatestLiveStreamByProgramIdAsync(programId);
            if (video == null || string.IsNullOrEmpty(video.URL))
                return NotFound(new { message = "No active livestream found for this program." });

            return Ok(new
            {
                video.VideoHistoryID,
                video.URL,
                video.PlaybackUrl,
                video.Type,
                video.Status,
                video.CreatedAt
            });
        }

        [HttpGet("by-date")]
        public async Task<IActionResult> GetVideosByDate([FromQuery] DateTime date)
        {
            var videos = await _videoService.GetVideosByDateAsync(date);

            var result = videos.Select(v => new
            {
                v.VideoHistoryID,
                v.Description,
                v.Type,
                v.URL,
                v.PlaybackUrl,
                v.MP4Url,
                v.Duration,
                CreatedAt = v.CreatedAt.ToString("HH:mm:ss"),
                v.Program?.ProgramName,
                v.ProgramID
            });

            return Ok(result);
        }
    }

}
