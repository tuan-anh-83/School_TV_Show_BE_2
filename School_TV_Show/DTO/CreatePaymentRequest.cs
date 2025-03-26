using System.ComponentModel.DataAnnotations;

namespace School_TV_Show.DTO
{
    public class CreatePaymentRequest
    {
        [Required(ErrorMessage = "OrderID is required.")]
        public int OrderID { get; set; }

        [Required(ErrorMessage = "Amount is required.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than 0.")]
        public decimal Amount { get; set; }

        [Required(ErrorMessage = "PaymentMethod is required.")]
        public string PaymentMethod { get; set; }
    }
}
