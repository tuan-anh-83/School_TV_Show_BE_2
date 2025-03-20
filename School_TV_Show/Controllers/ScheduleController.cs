using BOs.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using School_TV_Show.DTO;
using Services;

namespace School_TV_Show.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ScheduleController : ControllerBase
    {
        private readonly IScheduleService _scheduleService;

        public ScheduleController(IScheduleService scheduleService)
        {
            _scheduleService = scheduleService;
        }

        // GET: api/schedule/all
        [Authorize(Roles = "Admin,SchoolOwner,User")]
        [HttpGet("GetAllSchedules")]
        public async Task<IActionResult> GetAllSchedules()
        {
            try
            {
                var schedules = await _scheduleService.GetAllSchedulesAsync();
                if (schedules == null || !schedules.Any())
                    return NotFound("No schedules found.");

                var response = schedules.Select(s => new ScheduleResponse
                {
                    ScheduleID = s.ScheduleID,
                    StartTime = s.StartTime,
                    EndTime = s.EndTime,
                    Status = s.Status
                });
                return Ok(response);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving schedules: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        // GET: api/schedule/active
        [Authorize(Roles = "User,SchoolOwner,Admin")]
        [HttpGet("GetActiveSchedules")]
        public async Task<IActionResult> GetActiveSchedules()
        {
            try
            {
                var schedules = await _scheduleService.GetAllSchedulesAsync();
                if (schedules == null || !schedules.Any())
                    return NotFound("No schedules found.");

                var activeSchedules = schedules.Where(s => s.Status.Equals("Active", StringComparison.OrdinalIgnoreCase));
                if (!activeSchedules.Any())
                    return NotFound("No active schedules found.");

                var response = activeSchedules.Select(s => new ScheduleResponse
                {
                    ScheduleID = s.ScheduleID,
                    StartTime = s.StartTime,
                    EndTime = s.EndTime,
                    Status = s.Status
                });
                return Ok(response);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving active schedules: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        // GET: api/schedule/{id}
        [Authorize(Roles = "Admin,SchoolOwner")]
        [HttpGet("GetScheduleById/{id}")]
        public async Task<IActionResult> GetScheduleById(int id)
        {
            try
            {
                var schedule = await _scheduleService.GetScheduleByIdAsync(id);
                if (schedule == null)
                    return NotFound("Schedule not found");

                var response = new ScheduleResponse
                {
                    ScheduleID = schedule.ScheduleID,
                    StartTime = schedule.StartTime,
                    EndTime = schedule.EndTime,
                    Status = schedule.Status
                };
                return Ok(response);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving schedule: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        // POST: api/schedule
        [Authorize(Roles = "SchoolOwner")]
        [HttpPost("CreateSchedule")]
        public async Task<IActionResult> CreateSchedule([FromBody] CreateScheduleRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                Console.WriteLine("Received CreateSchedule request...");
                Console.WriteLine($"StartTime: {request.StartTime}, EndTime: {request.EndTime}");

                var schedule = new Schedule
                {
                    StartTime = request.StartTime,
                    EndTime = request.EndTime,
                    Status = "Active"
                };

                Console.WriteLine("Sending schedule to service...");
                var created = await _scheduleService.CreateScheduleAsync(schedule);

                var response = new ScheduleResponse
                {
                    ScheduleID = created.ScheduleID,
                    StartTime = created.StartTime,
                    EndTime = created.EndTime,
                    Status = created.Status
                };

                Console.WriteLine($"Schedule created successfully with ID: {created.ScheduleID}");
                return CreatedAtAction(nameof(GetScheduleById), new { id = created.ScheduleID }, response);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating schedule: {ex.Message}");
                if (ex.InnerException != null)
                    Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        // PUT: api/schedule/{id}
        [Authorize(Roles = "SchoolOwner")]
        [HttpPut("UpdateSchedule/{id}")]
        public async Task<IActionResult> UpdateSchedule(int id, [FromBody] UpdateScheduleRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var existingSchedule = await _scheduleService.GetScheduleByIdAsync(id);
                if (existingSchedule == null)
                    return NotFound("Schedule not found");

                if (!existingSchedule.Status.Equals("Active", StringComparison.OrdinalIgnoreCase))
                    return BadRequest("Only active schedules can be updated");

                existingSchedule.StartTime = request.StartTime;
                existingSchedule.EndTime = request.EndTime;

                var updated = await _scheduleService.UpdateScheduleAsync(existingSchedule);
                if (!updated)
                    return StatusCode(500, "Failed to update schedule");

                return NoContent();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating schedule: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        // DELETE: api/schedule/{id}
        [Authorize(Roles = "SchoolOwner,Admin")]
        [HttpDelete("DeleteSchedule/{id}")]
        public async Task<IActionResult> DeleteSchedule(int id)
        {
            try
            {
                var schedule = await _scheduleService.GetScheduleByIdAsync(id);
                if (schedule == null)
                    return NotFound("Schedule not found");

                var deleted = await _scheduleService.DeleteScheduleAsync(id);
                if (!deleted)
                    return StatusCode(500, "Failed to update schedule status");

                return NoContent();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating schedule status: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
