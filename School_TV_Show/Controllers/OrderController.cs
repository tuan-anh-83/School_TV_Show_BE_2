using BOs.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using School_TV_Show.DTO;
using Services;
using System.Security.Claims;

namespace School_TV_Show.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly IPackageService _packageService;
        private readonly IOrderDetailService _orderDetailService;
        public OrderController(IOrderService orderService, IPackageService packageService, IOrderDetailService orderDetailService)
        {
            _orderService = orderService;
            _packageService = packageService;
            _orderDetailService = orderDetailService;

        }

        [Authorize(Roles = "Admin")]
        [HttpGet("GetAllOrders")]
        public async Task<IActionResult> GetAllOrders()
        {
            try
            {
                var orders = await _orderService.GetAllOrdersAsync();
                return Ok(orders);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving orders: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrderById(int id)
        {
            var order = await _orderService.GetOrderByIdAsync(id);
            if (order == null)
            {
                return NotFound("Order not found.");
            }
            return Ok(order);
        }

        [Authorize(Roles = "SchoolOwner")]
        [HttpPost("CreateOrder")]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderRequestDTO request)
        {
            if (request == null || request.Quantity <= 0)
            {
                return BadRequest("Invalid order data.");
            }

            var accountIdClaim = User.FindFirst("AccountID")?.Value;
            if (!int.TryParse(accountIdClaim, out var accountId))
            {
                return Unauthorized("Token không hợp lệ, AccountID không đúng định dạng.");
            }

            var package = await _packageService.GetPackageByIdAsync(request.PackageID);
            if (package == null)
            {
                return NotFound("Package not found.");
            }

            decimal totalPrice = package.Price * request.Quantity;

            var newOrder = new Order
            {
                AccountID = accountId,
                TotalPrice = totalPrice,
                Status = "Pending",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            var createdOrder = await _orderService.CreateOrderAsync(newOrder);
            var orderDetail = new OrderDetail
            {
                OrderID = createdOrder.OrderID,
                PackageID = request.PackageID,
                Quantity = request.Quantity,
                Price = totalPrice
            };

            await _orderDetailService.CreateOrderDetailAsync(orderDetail);

            return CreatedAtAction(nameof(GetOrderById), new { id = createdOrder.OrderID }, createdOrder);
        }

        [HttpGet("GetOrderHistory")]
        [Authorize(Roles = "SchoolOwner")]
        public async Task<IActionResult> GetOrderHistory()
        {
            var accountIdClaim = User.FindFirst("AccountID")?.Value;
            if (!int.TryParse(accountIdClaim, out var accountId))
            {
                return Unauthorized("Token không hợp lệ, AccountID không đúng định dạng.");
            }

            var orders = await _orderService.GetOrdersByAccountIdAsync(accountId);
            if (orders == null || !orders.Any())
            {
                return NotFound(new { message = "No orders found for this account." });
            }

            return Ok(orders);

        }

    }

}
