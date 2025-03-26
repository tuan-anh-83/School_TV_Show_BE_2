using BOs.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using School_TV_Show.DTO;
using Services;
using System.Security.Claims;

namespace School_TV_Show.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VideoLikeController : ControllerBase
    {
        private readonly IVideoLikeService _videoLikeService;
        private readonly IVideoService _videoService;
        private readonly ILogger<VideoLikeController> _logger;

        public VideoLikeController(IVideoLikeService videoLikeService, IVideoService videoService, ILogger<VideoLikeController> logger)
        {
            _videoLikeService = videoLikeService;
            _videoService = videoService;
            _logger = logger;
        }

        // GET: api/videolike
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> GetAllVideoLikes()
        {
            try
            {
                var videoLikes = await _videoLikeService.GetAllVideoLikesAsync();
                return Ok(videoLikes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving video likes");
                return StatusCode(500, "Internal server error");
            }
        }
        // GET: api/videolike/positive
        [Authorize(Roles = "User,SchoolOwner")]
        [HttpGet("active")]
        public async Task<IActionResult> GetAllActiveVideoLikes()
        {
            try
            {
                var videoLikes = await _videoLikeService.GetAllVideoLikesAsync();
                var positiveVideoLikes = videoLikes.Where(v => v.Quantity > 0);
                return Ok(positiveVideoLikes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving positive video likes");
                return StatusCode(500, "Internal server error");
            }
        }

        // GET: api/videolike/{id}
        [Authorize(Roles = "Admin")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetVideoLikeById(int id)
        {
            try
            {
                var videoLike = await _videoLikeService.GetVideoLikeByIdAsync(id);
                if (videoLike == null)
                    return NotFound("Video like not found");

                return Ok(videoLike);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving video like by id");
                return StatusCode(500, "Internal server error");
            }
        }

        // GET: api/videolike/total/{videoHistoryId}
        [Authorize(Roles = "User,SchoolOwner")]
        [HttpGet("total/{videoHistoryId}")]
        public async Task<IActionResult> GetTotalLikesForVideo(int videoHistoryId)
        {
            try
            {
                var totalLikes = await _videoLikeService.GetTotalLikesForVideoAsync(videoHistoryId);
                return Ok(new { TotalLikes = totalLikes });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving total likes for video");
                return StatusCode(500, "Internal server error");
            }
        }
        // POST: api/videolike
        [Authorize(Roles = "User,SchoolOwner")]
        [HttpPost]
        public async Task<IActionResult> AddVideoLike([FromBody] CreateVideoLikeRequest request)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors)
                                              .Select(e => e.ErrorMessage)
                                              .ToList();
                return BadRequest(new { errors });
            }

            var accountIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (accountIdClaim == null)
            {
                return Unauthorized("User is not authenticated.");
            }

            if (!int.TryParse(accountIdClaim.Value, out int accountId))
            {
                return Unauthorized("Invalid account identifier.");
            }


            var videoLike = new VideoLike
            {
                VideoHistoryID = request.VideoHistoryID,
                AccountID = accountId,
                Quantity = 1
            };

            var video = await _videoService.GetVideoByIdAsync(request.VideoHistoryID);
            if (video == null || !video.Status)
            {
                return BadRequest("The specified video does not exist or is not active.");
            }

            try
            {
                var result = await _videoLikeService.AddVideoLikeAsync(videoLike);
                if (!result)
                {
                    return BadRequest("Invalid VideoHistoryID or AccountID.");
                }
                return CreatedAtAction(nameof(GetVideoLikeById), new { id = videoLike.LikeID }, videoLike);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating video like");
                return StatusCode(500, "Internal server error");
            }
        }

        // PUT: api/videolike/{id}
        [Authorize(Roles = "User,SchoolOwner")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateVideoLike(int id, [FromBody] UpdateVideoLikeRequest request)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors)
                                              .Select(e => e.ErrorMessage)
                                              .ToList();
                return BadRequest(new { errors });
            }

            var videoLike = await _videoLikeService.GetVideoLikeByIdAsync(id);
            if (videoLike == null)
            {
                return NotFound("Video like not found");
            }

            videoLike.Quantity = request.Quantity;

            try
            {
                var result = await _videoLikeService.UpdateVideoLikeAsync(videoLike);
                if (!result)
                {
                    return BadRequest("Invalid VideoHistoryID or AccountID.");
                }
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating video like");
                return StatusCode(500, "Internal server error");
            }
        }

        // DELETE: api/videolike/{id}
        [Authorize(Roles = "User,SchoolOwner")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteVideoLike(int id)
        {
            try
            {
                var result = await _videoLikeService.DeleteVideoLikeAsync(id);
                if (!result)
                    return NotFound("Video like not found");

                return Ok(new { message = "Video like deleted successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting video like");
                return StatusCode(500, "Internal server error");
            }
        }

        //  Tổng số lượt thích trên toàn bộ video
        [Authorize(Roles = "Admin")]
        [HttpGet("totalLikes")]
        public async Task<IActionResult> GetTotalLikes()
        {
            var totalLikes = await _videoLikeService.GetTotalLikesAsync();
            return Ok(new { totalLikes });
        }

        //  Tổng số lượt thích theo video
        [Authorize(Roles = "Admin")]
        [HttpGet("likesByVideo/{videoHistoryId}")]
        public async Task<IActionResult> GetLikesByVideo(int videoHistoryId)
        {
            var likes = await _videoLikeService.GetLikesByVideoIdAsync(videoHistoryId);
            return Ok(new { videoHistoryId, likes });
        }

        //  Tổng số lượt thích của từng video
        [Authorize(Roles = "Admin")]
        [HttpGet("likesPerVideo")]
        public async Task<IActionResult> GetLikesPerVideo()
        {
            var likesPerVideo = await _videoLikeService.GetLikesPerVideoAsync();
            return Ok(likesPerVideo);
        }
    }
}
