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
    public class ReportController : ControllerBase
    {
        private readonly IReportService _reportService;
        private readonly IAccountService _accountService;
        private readonly ILogger<ReportController> _logger;
        public ReportController(IReportService reportService, IAccountService accountService, ILogger<ReportController> logger)
        {
            _reportService = reportService;
            _accountService = accountService;
            _logger = logger;
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllReports()
        {
            try
            {
                var reports = await _reportService.GetAllReportsAsync();
                var response = reports.Select(r => new ReportResponse
                {
                    ReportID = r.ReportID,
                    AccountID = r.AccountID,
                    VideoHistoryID = r.VideoHistoryID,
                    Reason = r.Reason,
                    CreatedAt = r.CreatedAt,
                    Account = new AccountInfo
                    {
                        AccountID = r.Account?.AccountID ?? 0,
                        Username = r.Account?.Username
                    },
                    VideoHistory = new VideoHistoryInfo
                    {
                        VideoHistoryID = r.VideoHistory?.VideoHistoryID ?? 0,
                        URL = r.VideoHistory?.URL
                    }
                });

                return Ok(response);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving reports: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetReportById(int id)
        {
            try
            {
                var report = await _reportService.GetReportByIdAsync(id);
                if (report == null)
                    return NotFound("Report not found.");

                var response = new ReportResponse
                {
                    ReportID = report.ReportID,
                    AccountID = report.AccountID,
                    VideoHistoryID = report.VideoHistoryID,
                    Reason = report.Reason,
                    CreatedAt = report.CreatedAt,
                    Account = new AccountInfo
                    {
                        AccountID = report.Account?.AccountID ?? 0,
                        Username = report.Account?.Username
                    },
                    VideoHistory = new VideoHistoryInfo
                    {
                        VideoHistoryID = report.VideoHistory?.VideoHistoryID ?? 0,
                        URL = report.VideoHistory?.URL
                    }
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving report: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost]
        [Authorize(Roles = "User,SchoolOwner")]
        public async Task<IActionResult> CreateReport([FromBody] CreateReportRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var accountIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
                if (accountIdClaim == null || !int.TryParse(accountIdClaim.Value, out int accountId))
                {
                    return Unauthorized("Invalid token. Unable to retrieve account ID.");
                }
                var account = await _accountService.GetAccountByIdAsync(accountId);
                if (!account.Status.Equals("Active", StringComparison.OrdinalIgnoreCase))
                {
                    return Unauthorized("Your account is inactive. You cannot create a report.");
                }

                var newReport = new Report
                {
                    AccountID = accountId,
                    VideoHistoryID = request.VideoHistoryID,
                    Reason = request.Reason
                };

                var created = await _reportService.CreateReportAsync(newReport);

                var response = new ReportResponse
                {
                    ReportID = created.ReportID,
                    AccountID = created.AccountID,
                    VideoHistoryID = created.VideoHistoryID,
                    Reason = created.Reason,
                    CreatedAt = created.CreatedAt
                };

                return CreatedAtAction(nameof(GetReportById), new { id = created.ReportID }, response);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating report: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "User,SchoolOwner")]
        public async Task<IActionResult> UpdateReport(int id, [FromBody] UpdateReportRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var existingReport = await _reportService.GetReportByIdAsync(id);
                if (existingReport == null)
                    return NotFound("Report not found.");

                existingReport.Reason = request.Reason;

                var updated = await _reportService.UpdateReportAsync(existingReport);
                if (!updated)
                    return StatusCode(500, "Failed to update report.");

                return NoContent();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating report: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteReport(int id)
        {
            try
            {
                var deleted = await _reportService.DeleteReportAsync(id);
                if (!deleted)
                    return NotFound("Report not found or could not be deleted.");

                return NoContent();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting report: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("total-reports")]
        public async Task<IActionResult> GetTotalReports()
        {
            try
            {
                int totalReports = await _reportService.GetTotalReportsAsync();
                return Ok(new { TotalReports = totalReports });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving total reports.");
                return StatusCode(500, "Internal server error");
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("reports-per-video")]
        public async Task<IActionResult> GetReportsPerVideo()
        {
            try
            {
                Dictionary<int, int> reportsPerVideo = await _reportService.GetReportsPerVideoAsync();
                return Ok(reportsPerVideo);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving reports per video.");
                return StatusCode(500, "Internal server error");
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("all-reports")]
        public async Task<IActionResult> GetAllReport()
        {
            try
            {
                var reports = await _reportService.GetAllReportsAsync();
                return Ok(reports);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all reports.");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
