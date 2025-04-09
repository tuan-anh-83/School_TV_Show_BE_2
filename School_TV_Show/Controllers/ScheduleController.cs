using BOs.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using School_TV_Show.DTO;
using Services;

namespace School_TV_Show.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ScheduleController : ControllerBase
    {
        private readonly IScheduleService _scheduleService;
        private readonly IVideoService _videoHistoryService;
        private readonly CloudflareSettings _cloudflareSettings;

        public ScheduleController(IScheduleService scheduleService, IVideoService videoHistoryService, CloudflareSettings cloudflareSettings)
        {
            _scheduleService = scheduleService;
            _videoHistoryService = videoHistoryService;
            _cloudflareSettings = cloudflareSettings;
        }
        [HttpPost]
        public async Task<IActionResult> CreateSchedule([FromBody] CreateScheduleRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ApiResponse(false, "Invalid input"));

            var schedule = new Schedule
            {
                ProgramID = request.ProgramID,
                StartTime = request.StartTime,
                EndTime = request.EndTime,
                Status = "Pending",
                IsReplay = request.IsReplay
            };

            var created = await _scheduleService.CreateScheduleAsync(schedule);
            return Ok(new ApiResponse(true, "Schedule created", new { scheduleId = created.ScheduleID, isReplay = created.IsReplay }));
        }

        [HttpPost("replay-from-video")]
        public async Task<IActionResult> CreateReplayScheduleFromVideo([FromBody] CreateReplayScheduleRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ApiResponse(false, "Invalid input"));

            var video = await _videoHistoryService.GetVideoByIdAsync(request.VideoHistoryId);
            if (video == null || video.ProgramID == null)
                return NotFound(new ApiResponse(false, "Video not found or missing ProgramID"));

            var schedule = new Schedule
            {
                ProgramID = video.ProgramID.Value,
                StartTime = request.StartTime,
                EndTime = request.EndTime,
                Status = "Ready",
                IsReplay = true
            };

            var created = await _scheduleService.CreateScheduleAsync(schedule);

            var iframeUrl = string.IsNullOrEmpty(video.CloudflareStreamId)
            ? null
                : $"https://customer-{_cloudflareSettings.StreamDomain}.cloudflarestream.com/{video.CloudflareStreamId}/iframe";

            return Ok(new ApiResponse(true, "Replay schedule created", new
            {
                scheduleId = created.ScheduleID,
                videoId = video.VideoHistoryID,
                playbackUrl = video.PlaybackUrl,
                mp4Url = video.MP4Url,
                iframeUrl = iframeUrl,
                duration = video.Duration,
                description = video.Description,
                program = video.Program?.ProgramName ?? "No program",
                channel = video.Program?.SchoolChannel?.Name ?? "No channel"
            }));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetScheduleById(int id)
        {
            var schedule = await _scheduleService.GetScheduleByIdAsync(id);
            if (schedule == null)
                return NotFound(new ApiResponse(false, "Schedule not found"));

            return Ok(new ApiResponse(true, "Schedule found", schedule));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateSchedule(int id, [FromBody] UpdateScheduleRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ApiResponse(false, "Invalid input"));

            var existingSchedule = await _scheduleService.GetScheduleByIdAsync(id);
            if (existingSchedule == null)
                return NotFound(new ApiResponse(false, "Schedule not found"));

            if (!existingSchedule.Status.Equals("Pending", StringComparison.OrdinalIgnoreCase))
                return BadRequest(new ApiResponse(false, "Only 'Pending' schedules can be updated"));

            existingSchedule.StartTime = request.StartTime;
            existingSchedule.EndTime = request.EndTime;

            var updated = await _scheduleService.UpdateScheduleAsync(existingSchedule);
            return updated
                ? Ok(new ApiResponse(true, "Schedule updated successfully"))
                : StatusCode(500, new ApiResponse(false, "Failed to update schedule"));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSchedule(int id)
        {
            var schedule = await _scheduleService.GetScheduleByIdAsync(id);
            if (schedule == null)
                return NotFound(new ApiResponse(false, "Schedule not found"));

            if (!schedule.Status.Equals("Pending", StringComparison.OrdinalIgnoreCase))
                return BadRequest(new ApiResponse(false, "Only 'Pending' schedules can be deleted"));

            var deleted = await _scheduleService.DeleteScheduleAsync(id);
            return deleted
                ? Ok(new ApiResponse(true, "Schedule deleted successfully"))
                : StatusCode(500, new ApiResponse(false, "Failed to delete schedule"));
        }

        [HttpGet("by-channel-and-date")]
        [Authorize(Roles = "User,SchoolOwner,Admin")]
        public async Task<IActionResult> GetSchedulesByChannelAndDate([FromQuery] int channelId, [FromQuery] DateTime date)
        {
            if (channelId <= 0)
                return BadRequest(new ApiResponse(false, "Invalid channel ID"));

            var schedules = await _scheduleService.GetSchedulesByChannelAndDateAsync(channelId, date);
            var result = new List<object>();
            foreach (var schedule in schedules)
            {
                string? iframeUrl = null;
                string? playbackUrl = null;
                string? mp4Url = null;
                double? duration = null;
                string? description = null;
                int? videoHistoryId = null;

                if (schedule.IsReplay)
                {
                    var video = await _videoHistoryService.GetReplayVideoByProgramAndTimeAsync(schedule.ProgramID, schedule.StartTime, schedule.EndTime);
                    if (video != null)
                    {
                        iframeUrl = string.IsNullOrEmpty(video.CloudflareStreamId)
                            ? null
                            : $"https://customer-{_cloudflareSettings.StreamDomain}.cloudflarestream.com/{video.CloudflareStreamId}/iframe";

                        playbackUrl = video.PlaybackUrl;
                        mp4Url = video.MP4Url;
                        duration = video.Duration;
                        description = video.Description;
                        videoHistoryId = video.VideoHistoryID;
                    }
                }
                else
                {
                    iframeUrl = string.IsNullOrEmpty(schedule.Program?.CloudflareStreamId)
                        ? null
                        : $"https://customer-{_cloudflareSettings.StreamDomain}.cloudflarestream.com/{schedule.Program.CloudflareStreamId}/iframe";
                }

                result.Add(new
                {
                    schedule.ScheduleID,
                    schedule.StartTime,
                    schedule.EndTime,
                    schedule.Status,
                    schedule.IsReplay,
                    schedule.LiveStreamStarted,
                    schedule.LiveStreamEnded,
                    schedule.ProgramID,
                    iframeUrl,
                    playbackUrl,
                    mp4Url,
                    duration,
                    description,
                    videoHistoryId,
                    program = new
                    {
                        schedule.Program?.ProgramID,
                        schedule.Program?.ProgramName,
                        schedule.Program?.Title,
                        channel = schedule.Program?.SchoolChannel?.Name
                    }
                });
            }
            return Ok(new ApiResponse(true, "Schedules for channel and date", result));
        }

        [HttpGet("timeline")]
        public async Task<IActionResult> GetSchedulesTimeline()
        {
            var result = await _scheduleService.GetSchedulesGroupedTimelineAsync();
            return Ok(new ApiResponse(true, "Schedule timeline", result));
        }

        [HttpGet("by-date")]
        [Authorize(Roles = "User,SchoolOwner,Admin")]
        public async Task<IActionResult> GetSchedulesByDate([FromQuery] DateTime date)
        {
            var schedules = await _scheduleService.GetSchedulesByDateAsync(date);
            var result = schedules.Select(s => new
            {
                s.ScheduleID,
                s.StartTime,
                s.EndTime,
                s.Status,
                s.IsReplay,
                s.ProgramID,
                Program = new
                {
                    s.Program?.ProgramID,
                    s.Program?.ProgramName,
                    s.Program?.Title,
                    s.Program?.SchoolChannel?.Name
                }
            });

            return Ok(new ApiResponse(true, "Schedules by date", result));
        }
    }
}
