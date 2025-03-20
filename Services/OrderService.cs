using BOs.Models;
using Repos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepo _orderService;
        public OrderService(IOrderRepo orderService)
        {
            _orderService = orderService;
        }

        public async Task<Order> CreateOrderAsync(Order order)
        {
            return await _orderService.CreateOrderAsync(order);
        }

        public async Task<bool> DeleteOrderAsync(int orderId)
        {
            return await _orderService.DeleteOrderAsync(orderId);
        }

        public async Task<IEnumerable<Order>> GetAllOrdersAsync()
        {
            return await _orderService.GetAllOrdersAsync();
        }

        public async Task<Order> GetOrderByIdAsync(int orderId)
        {
            return await _orderService.GetOrderByIdAsync(orderId);
        }

        public async Task<IEnumerable<Order>> GetOrdersByAccountIdAsync(int accountId)
        {
           return await _orderService.GetOrdersByAccountIdAsync(accountId);
        }

        public async Task<bool> UpdateOrderAsync(Order order)
        {
            return await _orderService.UpdateOrderAsync(order);
        }
    }
}
