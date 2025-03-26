using System.ComponentModel.DataAnnotations;

namespace School_TV_Show.DTO
{
    public class PaymentRequest
    {
        [Required(ErrorMessage = "OrderID is required.")]
        public int OrderID { get; set; }

        [Required(ErrorMessage = "Amount is required.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than zero.")]
        public decimal Amount { get; set; }

        [Required(ErrorMessage = "PaymentMethod is required.")]
        [StringLength(100, ErrorMessage = "PaymentMethod cannot exceed 100 characters.")]
        public string PaymentMethod { get; set; } = string.Empty;

    }
}
