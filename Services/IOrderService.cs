using BOs.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Services
{
    public interface IOrderService
    {
        Task<Order> CreateOrderAsync(Order order);
        Task<Order> GetOrderByIdAsync(int orderId);
        Task<IEnumerable<Order>> GetAllOrdersAsync();
        Task<Order> UpdateOrderAsync(Order order);
        Task<IEnumerable<Order>> GetOrderHistoryAsync(int accountId);
        Task<object> GetOrderStatisticsAsync(DateTime? startDate, DateTime? endDate, string interval);

        Task<Order> GetOrderByOrderCodeAsync(long orderCode);
        Task<IEnumerable<Order>> GetPendingOrdersOlderThanAsync(TimeSpan timeSpan);
    }
}
