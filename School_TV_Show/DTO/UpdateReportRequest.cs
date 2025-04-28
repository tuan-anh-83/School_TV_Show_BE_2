using System.ComponentModel.DataAnnotations;

namespace School_TV_Show.DTO
{
    public class UpdateReportRequest
    {
        [Required(ErrorMessage = "Reason is required.")]
        [StringLength(500, ErrorMessage = "Reason cannot exceed 500 characters.")]
        public string Reason { get; set; }
    }
}
