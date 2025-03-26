using System.ComponentModel.DataAnnotations;

namespace School_TV_Show.DTO
{
    public class StreamScheduleRequest
    {
        [Required(ErrorMessage = "SchoolChannelID is required.")]
        public int SchoolChannelID { get; set; }

        [Required(ErrorMessage = "ProgramID is required.")]
        public int ProgramID { get; set; }

        [Required(ErrorMessage = "Start time is required.")]
        public DateTime StartTime { get; set; }

        [Required(ErrorMessage = "End time is required.")]
        public DateTime EndTime { get; set; }

        [Required(ErrorMessage = "Status is required.")]
        public string Status { get; set; } = "Scheduled";
    }
}
