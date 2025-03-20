using BOs.Models;
using DAOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repos
{
    public class OrderRepo : IOrderRepo
    {
        public async Task<Order> CreateOrderAsync(Order order)
        {
            return await OrderDAO.Instance.CreateOrderAsync(order);
        }

        public async Task<bool> DeleteOrderAsync(int orderId)
        {
            return await OrderDAO.Instance.DeleteOrderAsync(orderId);
        }

        public async Task<IEnumerable<Order>> GetAllOrdersAsync()
        {
            return await OrderDAO.Instance.GetAllOrdersAsync();
        }

        public async Task<Order> GetOrderByIdAsync(int orderId)
        {
            return await OrderDAO.Instance.GetOrderByIdAsync(orderId);
        }

        public async Task<IEnumerable<Order>> GetOrdersByAccountIdAsync(int accountId)
        {
            return await OrderDAO.Instance.GetOrdersByAccountIdAsync(accountId);
        }

        public async Task<bool> UpdateOrderAsync(Order order)
        {
            return await OrderDAO.Instance.UpdateOrderAsync(order);
        }
    }
}
