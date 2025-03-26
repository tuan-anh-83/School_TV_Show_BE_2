using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using School_TV_Show.DTO;
using Services;
using System.Security.Claims;

namespace School_TV_Show.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProgramController : ControllerBase
    {
        private readonly IProgramService _programService;
        private readonly ISchoolChannelService _schoolChannelService;
        private readonly IAccountService _accountService;
        private readonly IScheduleService _scheduleService;

        public ProgramController(IProgramService programService, ISchoolChannelService schoolChannelService, IAccountService accountService, IScheduleService scheduleService)
        {
            _scheduleService = scheduleService;
            _accountService = accountService;
            _schoolChannelService = schoolChannelService;
            _programService = programService;
        }

        [HttpGet("active")]
        public async Task<IActionResult> GetAllActivePrograms()
        {
            try
            {
                var programs = await _programService.GetAllProgramsAsync();
                var activePrograms = programs.Where(p => p.Status == "Active");
                var response = activePrograms.Select(p => new ProgramResponse
                {
                    ProgramID = p.ProgramID,
                    SchoolChannelID = p.SchoolChannelID,
                    ProgramName = p.ProgramName,
                    Title = p.Title,
                    Link = p.Link,
                    Status = p.Status,
                    CreatedAt = p.CreatedAt,
                    UpdatedAt = p.UpdatedAt,
                    Schedules = p.Schedules.Select(s => new ScheduleResponse
                    {
                        ScheduleID = s.ScheduleID,
                        StartTime = s.StartTime,
                        EndTime = s.EndTime
                    }).ToList(),
                    SchoolChannel = new SchoolChannelResponse
                    {
                        SchoolChannelID = p.SchoolChannel.SchoolChannelID,
                        Name = p.SchoolChannel.Name
                    }
                });
                return Ok(response);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving active programs: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> GetAllPrograms()
        {
            try
            {
                var programs = await _programService.GetAllProgramsAsync();
                var response = programs.Select(p => new ProgramResponse
                {
                    ProgramID = p.ProgramID,
                    SchoolChannelID = p.SchoolChannelID,
                    ProgramName = p.ProgramName,
                    Title = p.Title,
                    Link = p.Link,
                    Status = p.Status,
                    CreatedAt = p.CreatedAt,
                    UpdatedAt = p.UpdatedAt,
                    Schedules = p.Schedules.Select(s => new ScheduleResponse
                    {
                        ScheduleID = s.ScheduleID,
                        StartTime = s.StartTime,
                        EndTime = s.EndTime
                    }).ToList(),
                    SchoolChannel = new SchoolChannelResponse
                    {
                        SchoolChannelID = p.SchoolChannel.SchoolChannelID,
                        Name = p.SchoolChannel.Name
                    }
                });
                return Ok(response);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving programs: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        [Authorize(Roles = "SchoolOwner,Admin")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetProgramById(int id)
        {
            try
            {
                var program = await _programService.GetProgramByIdAsync(id);
                if (program == null)
                    return NotFound("Program not found");
                var response = new ProgramResponse
                {
                    ProgramID = program.ProgramID,
                    SchoolChannelID = program.SchoolChannelID,
                    ProgramName = program.ProgramName,
                    Title = program.Title,
                    Link = program.Link,
                    Status = program.Status,
                    CreatedAt = program.CreatedAt,
                    UpdatedAt = program.UpdatedAt,
                    Schedules = program.Schedules.Select(s => new ScheduleResponse
                    {
                        ScheduleID = s.ScheduleID,
                        StartTime = s.StartTime,
                        EndTime = s.EndTime
                    }).ToList(),
                    SchoolChannel = new SchoolChannelResponse
                    {
                        SchoolChannelID = program.SchoolChannel.SchoolChannelID,
                        Name = program.SchoolChannel.Name
                    }
                };
                return Ok(response);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving program: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchProgramsByName([FromQuery] string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return BadRequest("Name parameter is required.");
            try
            {
                var programs = await _programService.SearchProgramsByNameAsync(name);
                if (programs == null || !programs.Any())
                    return NotFound("No programs found matching the search criteria.");
                var response = programs.Select(p => new ProgramResponse
                {
                    ProgramID = p.ProgramID,
                    SchoolChannelID = p.SchoolChannelID,
                    ProgramName = p.ProgramName,
                    Title = p.Title,
                    Link = p.Link,
                    Status = p.Status,
                    CreatedAt = p.CreatedAt,
                    UpdatedAt = p.UpdatedAt,
                    Schedules = p.Schedules.Select(s => new ScheduleResponse
                    {
                        ScheduleID = s.ScheduleID,
                        StartTime = s.StartTime,
                        EndTime = s.EndTime
                    }).ToList(),
                    SchoolChannel = new SchoolChannelResponse
                    {
                        SchoolChannelID = p.SchoolChannel.SchoolChannelID,
                        Name = p.SchoolChannel.Name
                    }
                });
                return Ok(response);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error searching programs: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost]
        [Authorize(Roles = "SchoolOwner")]
        public async Task<IActionResult> CreateProgram([FromBody] CreateProgramRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var accountIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(accountIdClaim) || !int.TryParse(accountIdClaim, out int accountId))
            {
                return Unauthorized("Invalid token: Could not extract AccountID.");
            }
            var existingChannel = await _schoolChannelService.GetByIdAsync(request.SchoolChannelID);
            if (existingChannel == null)
                return BadRequest("Invalid SchoolChannelID: Channel does not exist.");
            if (existingChannel.AccountID != accountId)
                return Forbid("Unauthorized: You do not own this SchoolChannel.");
            var existingSchedule = await _scheduleService.GetScheduleByIdAsync(request.ScheduleID);
            if (existingSchedule == null)
                return BadRequest("Invalid ScheduleID: Schedule does not exist.");
            try
            {
                var newProgram = new BOs.Models.Program
                {
                    SchoolChannelID = request.SchoolChannelID,
                    ProgramName = request.ProgramName,
                    Title = request.Title,
                    Link = request.Link,
                    Status = "Active",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };
                var created = await _programService.CreateProgramAsync(newProgram);
                created.Schedules.Clear();
                created.Schedules.Add(existingSchedule);
                created.SchoolChannel = existingChannel;
                var response = new ProgramResponse
                {
                    ProgramID = created.ProgramID,
                    SchoolChannelID = created.SchoolChannelID,
                    ProgramName = created.ProgramName,
                    Title = created.Title,
                    Link = created.Link,
                    Status = created.Status,
                    CreatedAt = created.CreatedAt,
                    UpdatedAt = created.UpdatedAt,
                    Schedules = created.Schedules.Select(s => new ScheduleResponse
                    {
                        ScheduleID = s.ScheduleID,
                        StartTime = s.StartTime,
                        EndTime = s.EndTime
                    }).ToList(),
                    SchoolChannel = new SchoolChannelResponse
                    {
                        SchoolChannelID = created.SchoolChannel.SchoolChannelID,
                        Name = created.SchoolChannel.Name
                    }
                };
                return CreatedAtAction(nameof(GetProgramById), new { id = created.ProgramID }, response);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating program: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "SchoolOwner")]
        public async Task<IActionResult> UpdateProgram(int id, [FromBody] UpdateProgramRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            try
            {
                var existingProgram = await _programService.GetProgramByIdAsync(id);
                if (existingProgram == null)
                    return NotFound("Program not found");
                if (existingProgram.Status != "Active")
                    return BadRequest("Only active programs can be updated.");
                var newSchedule = await _scheduleService.GetScheduleByIdAsync(request.ScheduleID);
                if (newSchedule == null)
                    return BadRequest("Invalid ScheduleID: Schedule does not exist.");
                existingProgram.ProgramName = request.ProgramName;
                existingProgram.Title = request.Title;
                existingProgram.Link = request.Link;
                existingProgram.Schedules.Clear();
                existingProgram.Schedules.Add(newSchedule);
                existingProgram.UpdatedAt = DateTime.UtcNow;
                var updated = await _programService.UpdateProgramAsync(existingProgram);
                if (!updated)
                    return StatusCode(500, "Update failed");
                return NoContent();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating program: {ex.Message}");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "SchoolOwner,Admin")]
        public async Task<IActionResult> DeleteProgram(int id)
        {
            try
            {
                var program = await _programService.GetProgramByIdAsync(id);
                if (program == null)
                    return NotFound("Program not found");
                var deleted = await _programService.DeleteProgramAsync(id);
                if (!deleted)
                    return StatusCode(500, "Failed to delete program");
                return NoContent();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting program: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("totalPrograms")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetTotalPrograms()
        {
            var totalPrograms = await _programService.CountProgramsAsync();
            return Ok(new { totalPrograms });
        }

        [HttpGet("activePrograms")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetActivePrograms()
        {
            var activePrograms = await _programService.CountProgramsByStatusAsync("Active");
            return Ok(new { activePrograms });
        }

        [HttpGet("programsByStatus")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetProgramsByStatus()
        {
            var active = await _programService.CountProgramsByStatusAsync("Active");
            var pending = await _programService.CountProgramsByStatusAsync("Pending");
            var archived = await _programService.CountProgramsByStatusAsync("Archived");
            return Ok(new { active, pending, archived });
        }

        [HttpGet("programsBySchedule")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetProgramsBySchedule()
        {
            var schedules = await _scheduleService.GetAllSchedulesAsync();
            var data = schedules.Select(schedule => new
            {
                scheduleId = schedule.ScheduleID,
                startTime = schedule.StartTime,
                endTime = schedule.EndTime,
                programCount = _programService.CountProgramsByScheduleAsync(schedule.ScheduleID).Result
            });
            return Ok(data);
        }
    }
}
