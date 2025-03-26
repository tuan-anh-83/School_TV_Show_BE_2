using BOs.Models;
using Net.payOS.Types;
using System.Threading.Tasks;

namespace Repos
{
    public interface IPaymentRepo
    {
        Task<CreatePaymentResult> CreatePaymentAsync(Order order, string returnUrl, string cancelUrl);
        Task<Payment> GetPaymentByOrderIdAsync(int orderId);
        Task<Payment> UpdatePaymentAsync(Payment payment);
    }
}
