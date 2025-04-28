using BOs.Data;
using BOs.Models;
using Microsoft.EntityFrameworkCore;
using Net.payOS;
using Net.payOS.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DAOs
{
    public class PaymentDAO
    {
        private static PaymentDAO instance = null;
        private readonly DataContext _context;
        private readonly PayOS _payOS;

        private PaymentDAO(string clientId, string apiKey, string checksumKey, string? endpoint = null)
        {
            _context = new DataContext();
            _payOS = new PayOS(clientId, apiKey, checksumKey, endpoint); 
        }

        public static PaymentDAO Instance(string clientId, string apiKey, string checksumKey, string? endpoint = null)
        {
            if (instance == null)
            {
                instance = new PaymentDAO(clientId, apiKey, checksumKey, endpoint);
            }
            return instance;
        }

        public async Task<CreatePaymentResult> CreatePaymentAsync(Order order, string returnUrl, string cancelUrl)
        {
            if (order == null)
                throw new ArgumentNullException(nameof(order));

            List<ItemData> items = order.OrderDetails.Select(od =>
                new ItemData(od.Package.Name, (int)od.Quantity, (int)(od.Price))).ToList();

            PaymentData paymentData = new PaymentData(order.OrderID, (int)(order.TotalPrice),
                "Order Payment", items, cancelUrl, returnUrl);

            CreatePaymentResult paymentResult = await _payOS.createPaymentLink(paymentData);

            var existingPayment = await _context.Payments.FirstOrDefaultAsync(p => p.OrderID == order.OrderID);
            if (existingPayment == null)
            {
                var payment = new Payment
                {
                    OrderID = order.OrderID,
                    PaymentDate = DateTime.UtcNow,
                    Status = "Pending",
                    PaymentMethod = "Unknown",
                    Amount = order.TotalPrice
                };

                _context.Payments.Add(payment);
                await _context.SaveChangesAsync();
            }

            return paymentResult;
        }

        public async Task<Payment> GetPaymentByOrderIdAsync(int orderId)
        {
            if (orderId <= 0)
                throw new ArgumentException("Order ID must be greater than zero.");

            return await _context.Payments.FirstOrDefaultAsync(p => p.OrderID == orderId);
        }

        public async Task<Payment> UpdatePaymentAsync(Payment payment)
        {
            if (payment == null)
                throw new ArgumentNullException(nameof(payment));

            var existingPayment = await _context.Payments.FirstOrDefaultAsync(p => p.OrderID == payment.OrderID);

            if (existingPayment == null)
            {
                _context.Payments.Add(payment);
                Console.WriteLine($"✅ Payment inserted for Order {payment.OrderID}");
            }
            else
            {
                existingPayment.Status = payment.Status;
                existingPayment.PaymentMethod = payment.PaymentMethod;
                existingPayment.PaymentDate = payment.PaymentDate;
                existingPayment.Amount = payment.Amount;

                _context.Payments.Update(existingPayment);
                Console.WriteLine($"🔄 Payment updated for Order {payment.OrderID}");
            }

            await _context.SaveChangesAsync();
            return payment;
        }
    }
}
