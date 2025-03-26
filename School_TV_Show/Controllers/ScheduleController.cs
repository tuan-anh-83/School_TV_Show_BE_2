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
        private readonly IProgramService _programService;

        public ScheduleController(IScheduleService scheduleService, IProgramService programService)
        {
            _scheduleService = scheduleService;
            _programService = programService;
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("all")]
        public async Task<IActionResult> GetAllSchedules()
        {
            var schedules = await _scheduleService.GetAllSchedulesAsync();
            if (!schedules.Any())
                return NotFound(new { message = "No schedules found." });

            var response = schedules.Select(s => new ScheduleResponse
            {
                ScheduleID = s.ScheduleID,
                StartTime = s.StartTime,
                EndTime = s.EndTime,
                Status = s.Status,
                Mode = s.Mode,
                SourceVideoHistoryID = s.SourceVideoHistoryID
            });

            return Ok(response);
        }

        [Authorize(Roles = "User,SchoolOwner,Admin")]
        [HttpGet("active")]
        public async Task<IActionResult> GetActiveSchedules()
        {
            var activeSchedules = await _scheduleService.GetActiveSchedulesAsync();
            if (!activeSchedules.Any())
                return NotFound(new { message = "No active schedules found." });

            var response = activeSchedules.Select(s => new ScheduleResponse
            {
                ScheduleID = s.ScheduleID,
                StartTime = s.StartTime,
                EndTime = s.EndTime,
                Status = s.Status,
                Mode = s.Mode,
                SourceVideoHistoryID = s.SourceVideoHistoryID
            });
            return Ok(response);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetScheduleById(int id)
        {
            var schedule = await _scheduleService.GetScheduleByIdAsync(id);
            if (schedule == null)
                return NotFound(new { message = "Schedule not found." });

            var response = new ScheduleResponse
            {
                ScheduleID = schedule.ScheduleID,
                StartTime = schedule.StartTime,
                EndTime = schedule.EndTime,
                Status = schedule.Status,
                Mode = schedule.Mode,
                SourceVideoHistoryID = schedule.SourceVideoHistoryID
            };

            return Ok(response);
        }

        [Authorize(Roles = "SchoolOwner")]
        [HttpPost]
        public async Task<IActionResult> CreateSchedule([FromBody] CreateScheduleRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (request.StartTime >= request.EndTime)
                return BadRequest(new { error = "Start Time must be earlier than End Time." });

            var schedule = new Schedule
            {
                ProgramID = request.ProgramID,
                StartTime = request.StartTime,
                EndTime = request.EndTime,
                Status = "Active",
                Mode = request.Mode ?? "live",
                SourceVideoHistoryID = request.SourceVideoHistoryID
            };

            var createdSchedule = await _scheduleService.CreateScheduleAsync(schedule);

            var response = new ScheduleResponse
            {
                ScheduleID = createdSchedule.ScheduleID,
                StartTime = createdSchedule.StartTime,
                EndTime = createdSchedule.EndTime,
                Status = createdSchedule.Status,
                Mode = createdSchedule.Mode,
                SourceVideoHistoryID = createdSchedule.SourceVideoHistoryID
            };

            return CreatedAtAction(nameof(GetScheduleById), new { id = createdSchedule.ScheduleID }, response);
        }

        [Authorize(Roles = "SchoolOwner")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateSchedule(int id, [FromBody] UpdateScheduleRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var existingSchedule = await _scheduleService.GetScheduleByIdAsync(id);
            if (existingSchedule == null)
                return NotFound(new { message = "Schedule not found." });

            if (!existingSchedule.Status.Equals("Active", StringComparison.OrdinalIgnoreCase))
                return BadRequest(new { error = "Only active schedules can be updated." });

            existingSchedule.StartTime = request.StartTime;
            existingSchedule.EndTime = request.EndTime;

            var updated = await _scheduleService.UpdateScheduleAsync(existingSchedule);
            if (!updated)
                return StatusCode(500, new { error = "Failed to update schedule." });

            return Ok(new { message = "Schedule updated successfully." });
        }

        [Authorize(Roles = "SchoolOwner,Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSchedule(int id)
        {
            var schedule = await _scheduleService.GetScheduleByIdAsync(id);
            if (schedule == null)
                return NotFound(new { message = "Schedule not found." });

            var deleted = await _scheduleService.DeleteScheduleAsync(id);
            if (!deleted)
                return StatusCode(500, new { error = "Failed to delete schedule." });

            return Ok(new { message = "Schedule deleted successfully." });
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("dashboard/totalSchedules")]
        public async Task<IActionResult> GetTotalSchedules()
        {
            var totalSchedules = await _scheduleService.CountSchedulesAsync();
            return Ok(new { totalSchedules });
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("dashboard/activeSchedules")]
        public async Task<IActionResult> GetActiveSchedulesCount()
        {
            var activeSchedules = await _scheduleService.CountSchedulesByStatusAsync("Active");
            return Ok(new { activeSchedules });
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("dashboard/schedulesByStatus")]
        public async Task<IActionResult> GetSchedulesByStatus()
        {
            var result = await _scheduleService.GetScheduleCountByStatusAsync();
            return Ok(result);
        }

        [AllowAnonymous]
        [HttpGet("now-playing")]
        public async Task<IActionResult> GetNowPlayingSchedule()
        {
            var now = DateTime.UtcNow;

            var schedules = await _scheduleService.GetAllSchedulesAsync();
            var current = schedules
                .Where(s => s.Mode == "replay"
                    && s.SourceVideoHistoryID.HasValue
                    && s.StartTime <= now
                    && s.EndTime >= now
                    && s.Status == "Active")
                .FirstOrDefault();

            if (current == null)
                return NotFound(new { message = "Rigth now there is schedul for playback video." });

            var video = current.VideoHistory ?? current.Program?.VideoHistories
                ?.FirstOrDefault(v => v.VideoHistoryID == current.SourceVideoHistoryID.Value);

            if (video == null || string.IsNullOrEmpty(video.PlaybackUrl))
                return NotFound(new { message = "There is no video." });

            return Ok(new
            {
                scheduleId = current.ScheduleID,
                programId = current.ProgramID,
                sourceVideoId = current.SourceVideoHistoryID,
                playbackUrl = video.PlaybackUrl,
                description = video.Description,
                startTime = current.StartTime,
                endTime = current.EndTime,
                mode = current.Mode,
                type = "replay"
            });
        }
    }
}
