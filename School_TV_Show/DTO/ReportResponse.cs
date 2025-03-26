namespace School_TV_Show.DTO
{
    public class ReportResponse
    {
        public int ReportID { get; set; }
        public int AccountID { get; set; }
        public int VideoHistoryID { get; set; }
        public string Reason { get; set; }
        public DateTime CreatedAt { get; set; }

        public AccountInfo Account { get; set; }
        public VideoHistoryInfo VideoHistory { get; set; }
    }

    public class AccountInfo
    {
        public int AccountID { get; set; }
        public string Username { get; set; }
    }

    public class VideoHistoryInfo
    {
        public int VideoHistoryID { get; set; }
        public string URL { get; set; }
    }
}
