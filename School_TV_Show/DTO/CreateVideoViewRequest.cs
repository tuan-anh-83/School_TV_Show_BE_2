using System.ComponentModel.DataAnnotations;

namespace School_TV_Show.DTO
{
    public class CreateVideoViewRequest
    {
        [Required(ErrorMessage = "VideoHistoryID is required.")]
        public int VideoHistoryID { get; set; }
    }
}
