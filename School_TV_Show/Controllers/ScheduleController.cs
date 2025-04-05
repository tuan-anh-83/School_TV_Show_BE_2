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

        public ScheduleController(IScheduleService scheduleService, IVideoService videoHistoryService)
        {
            _scheduleService = scheduleService;
            _videoHistoryService = videoHistoryService;
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
                IsReplay = request.IsReplay // 🔥 Added
            };

            var created = await _scheduleService.CreateScheduleAsync(schedule);
            return Ok(new ApiResponse(true, "Schedule created", new { scheduleId = created.ScheduleID }));
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
        public async Task<IActionResult> GetSchedulesByChannelAndDate([FromQuery] int channelId, [FromQuery] DateTime date)
        {
            if (channelId <= 0)
                return BadRequest(new ApiResponse(false, "Invalid channel ID"));

            var schedules = await _scheduleService.GetSchedulesByChannelAndDateAsync(channelId, date);
            return Ok(new ApiResponse(true, "Schedules for channel and date", schedules));
        }

        [HttpGet("timeline")]
        public async Task<IActionResult> GetSchedulesByChannelAndDate()
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
                s.ProgramID,
                Program = new
                {
                    s.Program?.ProgramID,
                    s.Program?.ProgramName,
                    s.Program?.Title,
                    s.Program?.SchoolChannel?.Name
                }
            });

            return Ok(result);
        }
    }
}
