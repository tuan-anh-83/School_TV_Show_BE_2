namespace School_TV_Show.DTO
{
    public class ScheduleResponse
    {
        public int ScheduleID { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string Status { get; set; }
        public string Mode { get; set; }
        public int? SourceVideoHistoryID { get; set; }
    }
}
