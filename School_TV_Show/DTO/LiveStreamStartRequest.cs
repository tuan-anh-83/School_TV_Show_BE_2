using System.ComponentModel.DataAnnotations;

namespace School_TV_Show.DTO
{
    public class LiveStreamStartRequest
    {
        [Required(ErrorMessage = "ProgramID is required.")]
        public int ProgramID { get; set; }
        public string Description { get; set; } = string.Empty;
    }
}
