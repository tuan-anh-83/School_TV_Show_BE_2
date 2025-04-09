using BOs.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Repos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Services
{
    public class PaymentService : IPaymentService, IHostedService, IDisposable
    {
        private readonly IOrderService _orderService;
        private readonly IOrderDetailService _orderDetailService;
        private readonly IPaymentRepo _paymentRepo;
        private readonly IPaymentHistoryService _paymentHistoryService;
        private readonly string _checksumKey;
        private Timer _timer;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<PaymentService> _logger;
        private readonly IMembershipRepo _membershipRepo;

        public PaymentService(
            IOrderService orderService,
            IOrderDetailService orderDetailService,
            IPaymentRepo paymentRepo,
            IConfiguration configuration,
            IPaymentHistoryService paymentHistoryService,
            IServiceScopeFactory scopeFactory,
            ILogger<PaymentService> logger,
            IMembershipRepo membershipRepo)
        {
            _orderService = orderService;
            _orderDetailService = orderDetailService;
            _paymentRepo = paymentRepo;
            _paymentHistoryService = paymentHistoryService;
            _checksumKey = configuration["Environment:PAYOS_CHECKSUM_KEY"];
            _scopeFactory = scopeFactory;
            _logger = logger;
            _membershipRepo = membershipRepo;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("⏳ Starting Payment Status Checker...");
            _timer = new Timer(async _ => await MarkExpiredOrdersAsFailedAsync(), null, TimeSpan.Zero, TimeSpan.FromMinutes(5));
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("⏹️ Stopping Payment Status Checker...");
            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }

        private async Task MarkExpiredOrdersAsFailedAsync()
        {
            using var scope = _scopeFactory.CreateScope();
            var orderService = scope.ServiceProvider.GetRequiredService<IOrderService>();

            var expiredOrders = await orderService.GetPendingOrdersOlderThanAsync(TimeSpan.FromMinutes(15));

            foreach (var order in expiredOrders)
            {
                _logger.LogWarning($"⚠️ Order {order.OrderID} has exceeded 15 minutes without payment. Marking as 'Failed'.");

                order.Status = "Failed";
                await orderService.UpdateOrderAsync(order);
            }
        }

        public async Task<bool> HandlePaymentWebhookAsync(PayOSWebhookRequest request)
        {
            try
            {
                long orderCode = request.data.orderCode;
                _logger.LogInformation($"🔄 Processing payment webhook for OrderCode {orderCode}");

                var order = await _orderService.GetOrderByOrderCodeAsync(orderCode);
                if (order == null)
                {
                    _logger.LogError($"❌ Order with OrderCode {orderCode} not found.");
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
                    _logger.LogInformation($"✅ New payment record created for Order {order.OrderID}");
                }
                else
                {
                    if (payment.Status == "Completed" && request.data.code == "00")
                    {
                        _logger.LogWarning($"⚠️ Payment for Order {order.OrderID} is already completed. Skipping update.");
                        return true;
                    }

                    payment.Status = request.data.code == "00" ? "Completed" : "Failed";
                    await _paymentRepo.UpdatePaymentAsync(payment);
                    _logger.LogInformation($"✅ Payment record updated for Order {order.OrderID}");
                }

                await _paymentHistoryService.AddPaymentHistoryAsync(payment);
                _logger.LogInformation($"📜 Payment history recorded for Payment {payment.PaymentID}");

                order.Status = request.data.code == "00" ? "Completed" : "Failed";
                _logger.LogInformation($"✅ Order {order.OrderID} status updated to '{order.Status}'");

                await _orderService.UpdateOrderAsync(order);

                if (payment.Status == "Completed")
                {
                    var details = await _orderDetailService.GetOrderDetailsByOrderIdAsync(order.OrderID);
                    foreach (var detail in details)
                    {
                        var membership = new Membership
                        {
                            AccountID = order.AccountID,
                            PackageID = detail.PackageID,
                            StartDate = DateTime.UtcNow,
                            ExpirationDate = DateTime.UtcNow.AddDays(30),
                            RemainingDuration = detail.Package.Duration,
                            IsActive = true
                        };

                        var membershipService = _scopeFactory.CreateScope().ServiceProvider.GetRequiredService<IMembershipService>();
                        await membershipService.CreateMembershipAsync(membership);
                        _logger.LogInformation($"🎉 Membership created for Account {order.AccountID} with Package {detail.PackageID}");
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"❌ Error processing payment webhook: {ex.Message}");
                return false;
            }
        }

        public bool VerifySignature(PayOSWebhookRequest request)
        {
            try
            {
                _logger.LogInformation($"🔍 Verifying Signature for Order {request.data.orderCode}...");

                var sortedData = new SortedDictionary<string, string>();
                var dataProperties = request.data.GetType().GetProperties();
                foreach (var prop in dataProperties)
                {
                    var value = prop.GetValue(request.data, null)?.ToString() ?? "";
                    if (value.StartsWith("[") || value.StartsWith("{"))
                    {
                        value = JsonSerializer.Serialize(value);
                    }
                    sortedData[prop.Name] = value;
                }

                string dataString = string.Join("&", sortedData.Select(kv => $"{kv.Key}={kv.Value}"));
                _logger.LogInformation($"📝 Data String for Signature: {dataString}");

                using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(_checksumKey.Trim()));
                byte[] hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(dataString));
                string computedSignature = BitConverter.ToString(hash).Replace("-", "").ToLower();

                string receivedSignature = request.signature?.Trim().ToLower();
                bool isValid = computedSignature == receivedSignature;

                _logger.LogInformation($"🔐 Computed Signature: {computedSignature}");
                _logger.LogInformation($"📩 Received Signature: {receivedSignature}");
                _logger.LogInformation($"✅ Signature Match: {isValid}");

                return isValid;
            }
            catch (Exception ex)
            {
                _logger.LogError($"❌ Signature Verification Error: {ex.Message}");
                return false;
            }
        }
    }
}
