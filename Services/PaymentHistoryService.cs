using BOs.Models;
using Repos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class PaymentHistoryService : IPaymentHistoryService
    {
        private readonly IPaymentHistoryRepo _paymentHistoryRepo;

        public PaymentHistoryService(IPaymentHistoryRepo paymentHistoryRepo)
        {
            _paymentHistoryRepo = paymentHistoryRepo;
        }

        public async Task AddPaymentHistoryAsync(Payment payment)
        {
            await _paymentHistoryRepo.AddPaymentHistoryAsync(payment);
        }

        public async Task<List<PaymentHistory>> GetPaymentHistoriesByPaymentIdAsync(int paymentId)
        {
            return await _paymentHistoryRepo.GetPaymentHistoriesByPaymentIdAsync(paymentId);
        }

        public async Task<List<PaymentHistory>> GetAllPaymentHistoriesAsync()
        {
            return await _paymentHistoryRepo.GetAllPaymentHistoriesAsync();
        }

        public async Task<List<PaymentHistory>> GetPaymentHistoriesByUserIdAsync(int userId)
        {
            return await _paymentHistoryRepo.GetPaymentHistoriesByUserIdAsync(userId);
        }
    }
}
