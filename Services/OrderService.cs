using BOs.Models;
using Repos;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepo _orderRepo;

        public OrderService(IOrderRepo orderRepo)
        {
            _orderRepo = orderRepo;
        }

        public async Task<Order> CreateOrderAsync(Order order)
        {
            return await _orderRepo.CreateOrderAsync(order);
        }

        public async Task<Order> GetOrderByIdAsync(int orderId)
        {
            return await _orderRepo.GetOrderByIdAsync(orderId);
        }

        public async Task<IEnumerable<Order>> GetAllOrdersAsync()
        {
            return await _orderRepo.GetAllOrdersAsync();
        }
        public async Task<Order> UpdateOrderAsync(Order order)
        {
            return await _orderRepo.UpdateOrderAsync(order);
        }
        public async Task<IEnumerable<Order>> GetOrderHistoryAsync(int accountId)
        {
            return await _orderRepo.GetOrdersByAccountIdAsync(accountId);
        }
        public async Task<object> GetOrderStatisticsAsync(DateTime? startDate, DateTime? endDate, string interval)
        {
            return await _orderRepo.GetOrderStatisticsAsync(startDate, endDate, interval);
        }

        public async Task<Order> GetOrderByOrderCodeAsync(long orderCode)
        {
            return await _orderRepo.GetOrderByOrderCodeAsync(orderCode);
        }
        public async Task<IEnumerable<Order>> GetPendingOrdersOlderThanAsync(TimeSpan timeSpan)
        {
            return await _orderRepo.GetPendingOrdersOlderThanAsync(timeSpan);
        }
    }
}
