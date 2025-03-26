using BOs.Models;
using Microsoft.Extensions.Configuration;
using Repos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Services;

namespace Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IOrderService _orderService;
        private readonly IOrderDetailService _orderDetailService;
        private readonly IPaymentRepo _paymentRepo;
        private readonly IPaymentHistoryService _paymentHistoryService;
        private readonly string _checksumKey;

        public PaymentService(
            IOrderService orderService,
            IOrderDetailService orderDetailService,
            IPaymentRepo paymentRepo,
            IConfiguration configuration,
            IPaymentHistoryService paymentHistoryService)
        {
            _orderService = orderService;
            _orderDetailService = orderDetailService;
            _paymentRepo = paymentRepo;
            _paymentHistoryService = paymentHistoryService;
            _checksumKey = configuration["Environment:PAYOS_CHECKSUM_KEY"];


            Console.WriteLine($"🔑 Loaded Checksum Key: {_checksumKey}");
            _paymentHistoryService = paymentHistoryService;
        }

        public async Task<bool> HandlePaymentWebhookAsync(PayOSWebhookRequest request)
        {
            try
            {
                long orderCode = request.data.orderCode;
                Console.WriteLine($"🔄 Processing payment webhook for OrderCode {orderCode}");


                var order = await _orderService.GetOrderByOrderCodeAsync(orderCode);
                if (order == null)
                {
                    Console.WriteLine($"❌ Order with OrderCode {orderCode} not found in the database.");
                    return false;
                }

                var payment = await _paymentRepo.GetPaymentByOrderIdAsync(order.OrderID);
                if (payment == null)
                {
                    payment = new Payment
                    {
                        OrderID = order.OrderID,
                        Amount = request.data.amount,
                        PaymentDate = DateTime.UtcNow,
                        PaymentMethod = "PayOS",
                        Status = request.data.code == "00" ? "Completed" : "Failed"
                    };

                    await _paymentRepo.UpdatePaymentAsync(payment);
                    Console.WriteLine($"✅ New payment record created for Order {order.OrderID}");
                }
                else
                {
                    if (payment.Status == "Completed" && request.data.code == "00")
                    {
                        Console.WriteLine($"⚠️ Payment for Order {order.OrderID} is already completed. Skipping update.");
                        return true;
                    }

                    payment.Status = request.data.code == "00" ? "Completed" : "Failed";
                    await _paymentRepo.UpdatePaymentAsync(payment);
                    Console.WriteLine($"✅ Payment record updated for Order {order.OrderID}");
                }

                await _paymentHistoryService.AddPaymentHistoryAsync(payment);
                Console.WriteLine($"📜 Payment history recorded for Payment {payment.PaymentID}");


                if (request.data.code == "00")
                {
                    order.Status = "Completed";
                    Console.WriteLine($"✅ Order {order.OrderID} status updated to 'Completed'");
                }
                else
                {
                    order.Status = "Failed";
                    Console.WriteLine($"❌ Order {order.OrderID} status updated to 'Failed'");
                }

                await _orderService.UpdateOrderAsync(order);

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ [ERROR] Failed to process payment webhook for Order {request.data.orderCode}: {ex.Message}\nStackTrace: {ex.StackTrace}");
                return false;
            }
        }


        public bool VerifySignature(PayOSWebhookRequest request)
        {
            try
            {
                Console.WriteLine($"🔍 Verifying Signature for Order {request.data.orderCode}...");

                // ✅ Convert all request data into a sorted dictionary
                var sortedData = new SortedDictionary<string, string>();

                // ✅ Extract all fields dynamically (handles missing or extra fields)
                var dataProperties = request.data.GetType().GetProperties();
                foreach (var prop in dataProperties)
                {
                    var value = prop.GetValue(request.data, null)?.ToString() ?? "";

                    // ✅ JSON-encode arrays and objects properly
                    if (value.StartsWith("[") || value.StartsWith("{"))
                    {
                        value = JsonSerializer.Serialize(value);
                    }

                    sortedData[prop.Name] = value;
                }

                // ✅ Convert dictionary to the signature string format
                string dataString = string.Join("&", sortedData.Select(kv => $"{kv.Key}={kv.Value}"));
                Console.WriteLine($"📝 Data String for Signature: {dataString}");

                // ✅ Compute HMAC SHA256 hash
                using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(_checksumKey.Trim()));
                byte[] hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(dataString));
                string computedSignature = BitConverter.ToString(hash).Replace("-", "").ToLower();

                // ✅ Compare computed signature with received signature
                string receivedSignature = request.signature?.Trim().ToLower();
                bool isValid = computedSignature == receivedSignature;

                Console.WriteLine($"🔐 Computed Signature: {computedSignature}");
                Console.WriteLine($"📩 Received Signature: {receivedSignature}");
                Console.WriteLine($"✅ Signature Match: {isValid}");

                return isValid;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Signature Verification Error: {ex.Message}");
                return false;
            }
        }

    }
}
