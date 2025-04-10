using BOs.Models;
using DAOs;
using Net.payOS.Types;
using Repos.Interface;
using System.Threading.Tasks;

namespace Repos.Implements
{
    public class PaymentRepo : IPaymentRepo
    {
        private readonly PaymentDAO _paymentDAO;

        public PaymentRepo(string clientId, string apiKey, string checksumKey, string? endpoint = null)
        {
            _paymentDAO = PaymentDAO.Instance(clientId, apiKey, checksumKey, endpoint);
        }

        public async Task<CreatePaymentResult> CreatePaymentAsync(Order order, string returnUrl, string cancelUrl)
        {
            return await _paymentDAO.CreatePaymentAsync(order, returnUrl, cancelUrl);
        }

        public async Task<Payment> GetPaymentByOrderIdAsync(int orderId)
        {
            return await _paymentDAO.GetPaymentByOrderIdAsync(orderId);
        }

        public async Task<Payment> UpdatePaymentAsync(Payment payment)
        {
            return await _paymentDAO.UpdatePaymentAsync(payment);
        }
    }
}
