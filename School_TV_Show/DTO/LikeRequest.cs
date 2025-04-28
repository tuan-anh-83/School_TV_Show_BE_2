using System.ComponentModel.DataAnnotations;

namespace School_TV_Show.Controllers
{
    public class LikeRequest
    {
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1.")]
        public int Quantity { get; set; } = 1;
    }
}
