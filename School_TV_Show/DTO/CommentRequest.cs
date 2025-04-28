using System.ComponentModel.DataAnnotations;

namespace School_TV_Show.DTO
{
    public class CommentRequest
    {
        [Required(ErrorMessage = "Comment text is required.")]
        public string CommentText { get; set; } = string.Empty;
    }
}
