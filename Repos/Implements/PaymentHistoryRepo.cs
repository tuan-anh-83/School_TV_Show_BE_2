using BOs.Models;
using DAOs;
using Repos.Interface;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Repos.Implements
{
    public class PaymentHistoryRepo : IPaymentHistoryRepo
    {
        public async Task AddPaymentHistoryAsync(Payment payment)
        {
            await PaymentHistoryDAO.Instance.AddPaymentHistoryAsync(payment);
        }

        public async Task<List<PaymentHistory>> GetPaymentHistoriesByPaymentIdAsync(int paymentId)
        {
            return await PaymentHistoryDAO.Instance.GetPaymentHistoriesByPaymentIdAsync(paymentId);
        }

        public async Task<List<PaymentHistory>> GetAllPaymentHistoriesAsync()
        {
            return await PaymentHistoryDAO.Instance.GetAllPaymentHistoriesAsync();
        }

        public async Task<List<PaymentHistory>> GetPaymentHistoriesByUserIdAsync(int userId)
        {
            return await PaymentHistoryDAO.Instance.GetPaymentHistoriesByUserIdAsync(userId);
        }
    }
}
