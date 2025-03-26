using BOs.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Repos
{
    public interface IOrderRepo
    {
        Task<Order> CreateOrderAsync(Order order);
        Task<Order> GetOrderByIdAsync(int orderId);
        Task<IEnumerable<Order>> GetAllOrdersAsync();
        Task<Order> UpdateOrderAsync(Order order);
        Task<IEnumerable<Order>> GetOrdersByAccountIdAsync(int accountId);
        Task<object> GetOrderStatisticsAsync(DateTime? startDate, DateTime? endDate, string interval);

        Task<Order> GetOrderByOrderCodeAsync(long orderCode);
    }
}
