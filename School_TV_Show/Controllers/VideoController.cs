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
        private readonly ICloudService _cloudService;
        private readonly IVideoService _videoService;
        private readonly ILogger<VideoController> _logger;

        public VideoController(ICloudService cloudService, IVideoService videoService, ILogger<VideoController> logger)
        {
            _cloudService = cloudService;
            _videoService = videoService;
            _logger = logger;
        }

        [HttpPost("upload")]
       // [Authorize(Roles = "SchoolOwner")]
        public async Task<IActionResult> UploadVideo([FromForm] VideoUploadRequest request, IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest(new { message = "No video file uploaded" });

            try
            {
                var videoUrl = await _cloudService.UploadVideoAsync(file);

                var videoHistory = new VideoHistory
                {
                    Description = request.Description,
                    ProgramID = request.ProgramID,
                    Status = true,
                    Type = "mp4",
                    URL = videoUrl,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    StreamAt = DateTime.UtcNow
                };

                var result = await _videoService.AddVideoAsync(videoHistory);
                if (!result)
                    return StatusCode(500, "Error saving video metadata");

                return Ok(new { message = "Video uploaded successfully", videoUrl });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading video");
                return StatusCode(500, "Internal server error");
            }
        }
    }

}
