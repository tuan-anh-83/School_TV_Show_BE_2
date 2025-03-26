using System.ComponentModel.DataAnnotations;

namespace School_TV_Show.DTO
{
    public class CreateReportRequest
    {
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "VideoHistoryID must be a positive integer.")]
        public int VideoHistoryID { get; set; }

        [Required(ErrorMessage = "Reason is required.")]
        [StringLength(500, ErrorMessage = "Reason cannot exceed 500 characters.")]
        public string Reason { get; set; }
    }
}
