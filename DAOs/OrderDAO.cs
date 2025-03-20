using BOs.Data;
using BOs.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAOs
{
    public class OrderDAO
    {
        private static OrderDAO instance = null;
        private readonly DataContext context;

        private OrderDAO()
        {
            context = new DataContext();
        }

        public static OrderDAO Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new OrderDAO();
                }
                return instance;
            }
        }

        public async Task<Order> CreateOrderAsync(Order order)
        {
            context.Orders.Add(order);
            await context.SaveChangesAsync();
            return order;
        }

        public async Task<Order> GetOrderByIdAsync(int orderId)
        {
            return await context.Orders
            .Include(o => o.OrderDetails)
                .ThenInclude(od => od.Package)
            .FirstOrDefaultAsync(o => o.OrderID == orderId);
        }

        public async Task<IEnumerable<Order>> GetOrdersByAccountIdAsync(int accountId)
        {
            return await context.Orders
                .Where(o => o.AccountID == accountId)
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Package)
                .ToListAsync();
        }

        public async Task<IEnumerable<Order>> GetAllOrdersAsync()
        {
            return await context.Orders
            .Include(o => o.OrderDetails)
                .ThenInclude(od => od.Package)
            .ToListAsync();
        }

        public async Task<bool> UpdateOrderAsync(Order order)
        {
            context.Orders.Update(order);
            return await context.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteOrderAsync(int orderId)
        {
            var order = await context.Orders.FindAsync(orderId);
            if (order == null) return false;

            context.Orders.Remove(order);
            return await context.SaveChangesAsync() > 0;
        }
    }
}
