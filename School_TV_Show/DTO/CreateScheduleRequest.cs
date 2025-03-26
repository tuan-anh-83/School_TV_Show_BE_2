using System.ComponentModel.DataAnnotations;

namespace School_TV_Show.DTO
{
    public class CreateScheduleRequest
    {
        public int ProgramID { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string? Mode { get; set; }
        public int? SourceVideoHistoryID { get; set; }
    }
}
