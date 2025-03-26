using System.ComponentModel.DataAnnotations;

namespace School_TV_Show.DTO
{
    public class LiveStreamStartRequest
    {
        [Required(ErrorMessage = "ProgramID is required.")]
        public int ProgramID { get; set; }

        [Required(ErrorMessage = "Stream URL is required.")]
        public string StreamUrl { get; set; } = string.Empty;

        [Required(ErrorMessage = "Stream type is required.")]
        public string StreamType { get; set; } = "Live";
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }
}
