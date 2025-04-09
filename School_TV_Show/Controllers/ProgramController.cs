﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using School_TV_Show.DTO;
using Services;
using System.Security.Claims;
using BOs.Models;
using Services;

namespace School_TV_Show.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProgramController : ControllerBase
    {
        private readonly IProgramService _programService;
        private readonly ISchoolChannelService _schoolChannelService;

        public ProgramController(IProgramService programService, ISchoolChannelService schoolChannelService)
        {
            _programService = programService;
            _schoolChannelService = schoolChannelService;
        }
        [HttpGet("with-videos")]
        public async Task<IActionResult> GetProgramsWithVideos()
        {
            var programs = await _programService.GetProgramsWithVideosAsync();
            return Ok(programs.Select(p => new { p.ProgramID, p.Title }));
        }

        [HttpGet("without-videos")]
        public async Task<IActionResult> GetProgramsWithoutVideos()
        {
            var programs = await _programService.GetProgramsWithoutVideosAsync();
            return Ok(programs.Select(p => new { p.ProgramID, p.Title }));
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAllPrograms()
        {
            var programs = await _programService.GetAllProgramsAsync();

            var result = programs.Select(p => new ProgramResponse
            {
                ProgramID = p.ProgramID,
                ProgramName = p.ProgramName,
                Title = p.Title,
                Status = p.Status,
                CreatedAt = p.CreatedAt,
                UpdatedAt = p.UpdatedAt,
                SchoolChannelID = p.SchoolChannelID,
                SchoolChannel = p.SchoolChannel == null ? null : new SchoolChannelResponse
                {
                    SchoolChannelID = p.SchoolChannel.SchoolChannelID,
                    Name = p.SchoolChannel.Name,
                    Description = p.SchoolChannel.Description,
                    Website = p.SchoolChannel.Website,
                    Email = p.SchoolChannel.Email,
                    Address = p.SchoolChannel.Address
                },
                Schedules = p.Schedules?.Select(s => new ScheduleResponse
                {
                    ScheduleID = s.ScheduleID,
                    StartTime = s.StartTime,
                    EndTime = s.EndTime,
                    Status = s.Status,
                    LiveStreamStarted = s.LiveStreamStarted,
                    LiveStreamEnded = s.LiveStreamEnded,
                    ProgramID = s.ProgramID
                }).ToList()
            });

            return Ok(new ApiResponse(true, "All programs with schedule & channel", result));
        }

        [HttpPost]
        public async Task<IActionResult> CreateProgram([FromBody] CreateProgramRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ApiResponse(false, "Invalid input", ModelState));

            var program = new BOs.Models.Program
            {
                ProgramName = request.ProgramName,
                Title = request.Title,
                Status = "Active",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                SchoolChannelID = request.SchoolChannelID
            };

            var created = await _programService.CreateProgramAsync(program);
            return Ok(new ApiResponse(true, "Program created", new { programId = created.ProgramID }));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProgram(int id, [FromBody] UpdateProgramRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ApiResponse(false, "Invalid input", ModelState));

            var program = await _programService.GetProgramByIdAsync(id);
            if (program == null)
                return NotFound(new ApiResponse(false, "Program not found"));

            program.ProgramName = request.ProgramName;
            program.Title = request.Title;
            program.UpdatedAt = DateTime.UtcNow;

            var success = await _programService.UpdateProgramAsync(program);
            if (!success)
                return StatusCode(500, new ApiResponse(false, "Failed to update program"));

            return Ok(new ApiResponse(true, "Program updated successfully"));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProgram(int id)
        {
            var program = await _programService.GetProgramByIdAsync(id);
            if (program == null)
                return NotFound(new ApiResponse(false, "Program not found"));

            var success = await _programService.DeleteProgramAsync(id);
            if (!success)
                return StatusCode(500, new ApiResponse(false, "Failed to delete program"));

            return Ok(new ApiResponse(true, "Program deleted successfully"));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetProgramById(int id)
        {
            var p = await _programService.GetProgramByIdAsync(id);
            if (p == null)
                return NotFound(new ApiResponse(false, "Program not found"));

            var response = new ProgramResponse
            {
                ProgramID = p.ProgramID,
                ProgramName = p.ProgramName,
                Title = p.Title,
                Status = p.Status,
                CreatedAt = p.CreatedAt,
                UpdatedAt = p.UpdatedAt,
                SchoolChannelID = p.SchoolChannelID,
                SchoolChannel = p.SchoolChannel == null ? null : new SchoolChannelResponse
                {
                    SchoolChannelID = p.SchoolChannel.SchoolChannelID,
                    Name = p.SchoolChannel.Name,
                    Description = p.SchoolChannel.Description,
                    Website = p.SchoolChannel.Website,
                    Email = p.SchoolChannel.Email,
                    Address = p.SchoolChannel.Address
                },
                Schedules = p.Schedules?.Select(s => new ScheduleResponse
                {
                    ScheduleID = s.ScheduleID,
                    StartTime = s.StartTime,
                    EndTime = s.EndTime,
                    Status = s.Status,
                    LiveStreamStarted = s.LiveStreamStarted,
                    LiveStreamEnded = s.LiveStreamEnded,
                    ProgramID = s.ProgramID
                }).ToList()
            };

            return Ok(new ApiResponse(true, "Program found", response));
        }
        [HttpGet("by-channel/{channelId}")]
        [Authorize(Roles = "User,SchoolOwner,Admin")]
        public async Task<IActionResult> GetProgramsByChannel(int channelId)
        {
            var programs = await _programService.GetProgramsByChannelIdAsync(channelId);

            var result = programs.Select(p => new ProgramResponse
            {
                ProgramID = p.ProgramID,
                ProgramName = p.ProgramName,
                Title = p.Title,
                Status = p.Status,
                CreatedAt = p.CreatedAt,
                UpdatedAt = p.UpdatedAt,
                SchoolChannelID = p.SchoolChannelID,
                SchoolChannel = p.SchoolChannel == null ? null : new SchoolChannelResponse
                {
                    SchoolChannelID = p.SchoolChannel.SchoolChannelID,
                    Name = p.SchoolChannel.Name,
                    Description = p.SchoolChannel.Description,
                    Website = p.SchoolChannel.Website,
                    Email = p.SchoolChannel.Email,
                    Address = p.SchoolChannel.Address
                },
                Schedules = p.Schedules?.Select(s => new ScheduleResponse
                {
                    ScheduleID = s.ScheduleID,
                    StartTime = s.StartTime,
                    EndTime = s.EndTime,
                    Status = s.Status,
                    LiveStreamStarted = s.LiveStreamStarted,
                    LiveStreamEnded = s.LiveStreamEnded,
                    ProgramID = s.ProgramID
                }).ToList()
            });

            return Ok(new ApiResponse(true, "Programs by channel", result));
        }
    }
}
