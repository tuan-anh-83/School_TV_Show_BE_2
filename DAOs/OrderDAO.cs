using BOs.Data;
using BOs.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Globalization;
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
            var existingOrder = await context.Orders
                .FirstOrDefaultAsync(o => o.AccountID == order.AccountID && o.Status == "Pending");

            if (existingOrder != null)
            {
                throw new Exception("Đơn thanh toán đã tồn tại");
            }

            order.OrderCode = GenerateUniqueOrderCode();
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

        public async Task<IEnumerable<Order>> GetAllOrdersAsync()
        {
            return await context.Orders
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Package)
                .ToListAsync();
        }

        public async Task<Order> UpdateOrderAsync(Order order)
        {
            context.Orders.Update(order);
            await context.SaveChangesAsync();
            return order;
        }
        public async Task<IEnumerable<Order>> GetOrdersByAccountIdAsync(int accountId)
        {
            return await context.Orders
                .Where(o => o.AccountID == accountId)
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Package)
                .ToListAsync();
        }
        public async Task<object> GetOrderStatisticsAsync(DateTime? startDate, DateTime? endDate, string interval)
        {
            var query = context.Orders.AsQueryable();

            if (startDate.HasValue && endDate.HasValue)
            {
                query = query.Where(o => o.CreatedAt >= startDate.Value && o.CreatedAt <= endDate.Value);
            }

            var totalOrders = await query.CountAsync();
            var totalRevenue = await query.SumAsync(o => o.TotalPrice);

            var orderStatusCounts = await query
                .GroupBy(o => o.Status)
                .Select(g => new { Status = g.Key, Count = g.Count() })
                .ToListAsync();

            var segmentedOrders = query.AsEnumerable()
                .GroupBy(o => interval.ToLower() switch
                {
                    "daily" => o.CreatedAt.Date.ToString("yyyy-MM-dd"),
                    "weekly" => $"{o.CreatedAt.Year}-W{CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(o.CreatedAt, CalendarWeekRule.FirstDay, DayOfWeek.Monday)}",
                    "monthly" => $"{o.CreatedAt.Year}-{o.CreatedAt.Month:D2}",
                    "yearly" => o.CreatedAt.Year.ToString(),
                    _ => o.CreatedAt.Date.ToString("yyyy-MM-dd")
                })
                .Select(g => new
                {
                    Period = g.Key,
                    OrderCount = g.Count(),
                    Revenue = g.Sum(o => o.TotalPrice)
                })
                .ToList();

            return new
            {
                TotalOrders = totalOrders,
                TotalRevenue = totalRevenue,
                OrdersByStatus = orderStatusCounts.ToDictionary(x => x.Status, x => x.Count),
                SegmentedOrders = segmentedOrders
            };
        }

        public async Task<Order> GetOrderByOrderCodeAsync(long orderCode)
        {
            return await context.Orders
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Package)
                .FirstOrDefaultAsync(o => o.OrderCode == orderCode);
        }
        private long GenerateUniqueOrderCode()
        {
            return DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        }
    }
}
