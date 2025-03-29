using System.ComponentModel.DataAnnotations;

namespace School_TV_Show.DTO
{
    public class UpdateScheduleRequest
    {
        [Required(ErrorMessage = "StartTime is required.")]
        public DateTime StartTime { get; set; }

        [Required(ErrorMessage = "EndTime is required.")]
        public DateTime EndTime { get; set; }

        [StringLength(20, ErrorMessage = "Mode cannot exceed 20 characters.")]
        public string? Mode { get; set; }
    }
}
