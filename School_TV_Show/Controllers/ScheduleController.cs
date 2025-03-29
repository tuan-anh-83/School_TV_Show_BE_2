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
        [HttpPost("manual-replay")]
        [Authorize(Roles = "SchoolOwner,Admin")]
        public async Task<IActionResult> CreateManualReplaySchedule([FromBody] CreateReplayScheduleRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ApiResponse(false, "Invalid request"));

            var start = request.StartTime ?? DateTime.UtcNow;
            var end = request.EndTime ?? start.AddMinutes(5);

            var schedule = new Schedule
            {
                ProgramID = request.ProgramID,
                StartTime = start,
                EndTime = end,
                Status = "Active",
                Mode = "replay",

            };

            var result = await _scheduleService.CreateScheduleAsync(schedule);

            if (schedule == null)
                return StatusCode(500, new ApiResponse(false, "Failed to create replay schedule"));

            return Ok(new ApiResponse(true, "Replay scheduled successfully", new
            {
                schedule.ScheduleID,
                schedule.ProgramID,
                schedule.Mode,
                schedule.StartTime,
                schedule.EndTime
            }));
        }

        [HttpPost("replay")]
        public async Task<IActionResult> CreateReplaySchedule([FromBody] CreateReplayScheduleRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ApiResponse(false, "Invalid input"));

            var start = request.StartTime ?? DateTime.UtcNow;
            var end = request.EndTime ?? start.AddMinutes(5);

            var schedule = new Schedule
            {
                ProgramID = request.ProgramID,
                StartTime = start,
                EndTime = end,
                Mode = "replay",
                Status = "Active",
            };

            var success = await _scheduleService.CreateScheduleAsync(schedule);
            if (schedule == null)
                return StatusCode(500, new ApiResponse(false, "Failed to create replay schedule"));

            return Ok(new ApiResponse(true, "Replay schedule created", new
            {
                schedule.ScheduleID,
                schedule.ProgramID,
                schedule.StartTime,
                schedule.EndTime,
                schedule.Mode,
            }));
        }

        [HttpGet("dashboard/status-counts")]
        public async Task<IActionResult> GetScheduleDashboardStatusCounts()
        {
            var counts = await _scheduleService.GetScheduleCountByStatusAsync();
            return Ok(new ApiResponse(true, "Schedule status summary", counts));
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAllSchedules()
        {
            var schedules = await _scheduleService.GetAllSchedulesAsync();
            return Ok(new ApiResponse(true, "All schedules", schedules));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetScheduleById(int id)
        {
            var schedule = await _scheduleService.GetScheduleByIdAsync(id);
            if (schedule == null)
                return NotFound(new ApiResponse(false, "Schedule not found"));

            return Ok(new ApiResponse(true, "Schedule found", schedule));
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
                Mode = request.Mode,
                Status = "Pending",
            };

            var created = await _scheduleService.CreateScheduleAsync(schedule);
            return Ok(new ApiResponse(true, "Schedule created", new { scheduleId = created.ScheduleID }));
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
            existingSchedule.Mode = request.Mode;

            var updated = await _scheduleService.UpdateScheduleAsync(existingSchedule);
            if (!updated)
                return StatusCode(500, new ApiResponse(false, "Failed to update schedule"));

            return Ok(new ApiResponse(true, "Schedule updated successfully"));
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
            if (!deleted)
                return StatusCode(500, new ApiResponse(false, "Failed to delete schedule"));

            return Ok(new ApiResponse(true, "Schedule deleted successfully"));
        }

        [HttpGet("status/{status}")]
        public async Task<IActionResult> GetSchedulesByStatus(string status)
        {
            var result = await _scheduleService.GetSchedulesByStatusAsync(status);
            return Ok(new ApiResponse(true, $"Schedules with status '{status}'", result));
        }

        [HttpGet("count-by-status/{status}")]
        public async Task<IActionResult> GetScheduleCountByStatus(string status)
        {
            var count = await _scheduleService.CountSchedulesByStatusAsync(status);
            return Ok(new ApiResponse(true, $"Count of schedules with status '{status}'", new { status, count }));
        }

        [HttpGet("count-by-status")]
        public async Task<IActionResult> GetScheduleCountsByStatus()
        {
            var result = await _scheduleService.GetScheduleCountByStatusAsync();
            return Ok(new ApiResponse(true, "All status counts", result));
        }
    }
}
