using System.ComponentModel.DataAnnotations;

namespace School_TV_Show.DTO
{
    public class ProgramRequest
    {
        [Required(ErrorMessage = "ScheduleID is required.")]
        public int ScheduleID { get; set; }

        [Required(ErrorMessage = "SchoolChannelID is required.")]
        public int SchoolChannelID { get; set; }

        [Required(ErrorMessage = "Program name is required.")]
        [StringLength(100, ErrorMessage = "Program name cannot be longer than 100 characters.")]
        public string ProgramName { get; set; } = string.Empty;

        public string Title { get; set; } = string.Empty;
        public string Link { get; set; } = string.Empty;

        [Required(ErrorMessage = "Status is required.")]
        public string Status { get; set; } = "Active";
    }
}
