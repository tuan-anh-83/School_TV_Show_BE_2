using BOs.Models;
using Microsoft.AspNetCore.Mvc;
using School_TV_Show.DTO;
using Services;

namespace School_TV_Show.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PackageController : ControllerBase
    {
        private readonly IPackageService _packageService;
        private readonly ILogger<PackageController> _logger;

        public PackageController(IPackageService packageService, ILogger<PackageController> logger)
        {
            _packageService = packageService;
            _logger = logger;
        }

        [HttpGet("GetAllPackages")]
        public async Task<IActionResult> GetAllPackages()
        {
            try
            {
                var packages = await _packageService.GetAllPackagesAsync();
                return Ok(packages);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving packages");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("GetActivePackages")]
        public async Task<IActionResult> GetActivePackages()
        {
            try
            {
                var packages = await _packageService.GetActivePackagesAsync();
                return Ok(packages);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving packages");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("GetPackageById{id}")]
        public async Task<IActionResult> GetPackageById(int id)
        {
            try
            {
                var package = await _packageService.GetPackageByIdAsync(id);
                if (package == null)
                    return NotFound("Package not found");

                return Ok(package);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving package by id");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("AddPackage")]
        public async Task<IActionResult> AddPackage([FromBody] CreatePackageRequestDTO request)
        {
            if (request == null)
                return BadRequest("Invalid package data.");

            var package = new Package
            {
                Name = request.Name,
                Description = request.Description,
                Price = request.Price,
                Duration = request.Duration,
                Status = "Active", 
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            try
            {
                await _packageService.AddPackageAsync(package);
                return CreatedAtAction(nameof(GetPackageById), new { id = package.PackageID }, package);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating package");
                return StatusCode(500, "Internal server error");
            }
        }


        [HttpPut("UpdatePackage{id}")]
        public async Task<IActionResult> UpdatePackage(int id, [FromBody] UpdatePackageRequestDTO request)
        {
            try
            {
                var existingPackage = await _packageService.GetPackageByIdAsync(id);
                if (existingPackage == null)
                    return NotFound("Package not found");

                existingPackage.Name = request.Name;
                existingPackage.Description = request.Description;
                existingPackage.Price = request.Price;
                existingPackage.Duration = request.Duration;
                existingPackage.UpdatedAt = DateTime.UtcNow; 

                bool isUpdated = await _packageService.UpdatePackageAsync(existingPackage);
                if (!isUpdated)
                    return StatusCode(500, "Failed to update package");

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating package");
                return StatusCode(500, "Internal server error");
            }
        }


        [HttpDelete("DeletePackage{id}")]
        public async Task<IActionResult> DeletePackage(int id)
        {
            try
            {
                var result = await _packageService.DeletePackageAsync(id);
                if (!result)
                    return NotFound("Package not found");

                return Ok(new { message = "Package marked as inactive successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting package");
                return StatusCode(500, "Internal server error");
            }
        }

    }
}
