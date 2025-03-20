using System.ComponentModel.DataAnnotations;

namespace School_TV_Show.DTO
{
    public class UpdateSchoolChannelRequestDTO
    {
        [Required(ErrorMessage = "Name is required.")]
        [StringLength(255, ErrorMessage = "Name cannot exceed 255 characters.")]
        public string Name { get; set; } = string.Empty;

        [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters.")]
        public string? Description { get; set; }

        [StringLength(255, ErrorMessage = "Address cannot exceed 255 characters.")]
        public string? Address { get; set; }

        [StringLength(255, ErrorMessage = "Email cannot exceed 255 characters.")]
        public string? Email { get; set; }

        [StringLength(255, ErrorMessage = "Website cannot exceed 255 characters.")]
        public string? Website { get; set; }
    }
}
