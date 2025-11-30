using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using OrderManagementApi.Data;
using OrderManagementApi.Models;

namespace OrderManagementApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class OrdersController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        public OrdersController(ApplicationDbContext db) { _db = db; }

        private string CurrentUserId => User.FindFirstValue(ClaimTypes.NameIdentifier);

        [HttpGet]
        public async Task<IActionResult> GetOrders()
        {
            var uid = CurrentUserId;
            var orders = await _db.Orders.Where(o => o.UserId == uid).ToListAsync();
            return Ok(orders);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrder(int id)
        {
            var uid = CurrentUserId;
            var order = await _db.Orders.FirstOrDefaultAsync(o => o.Id == id && o.UserId == uid);
            if (order == null) return NotFound();
            return Ok(order);
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] OrderCreateDto dto)
        {
            var uid = CurrentUserId;
            var order = new Order
            {
                ProductName = dto.ProductName,
                Quantity = dto.Quantity,
                UnitPrice = dto.UnitPrice,
                TotalAmount = dto.Quantity * dto.UnitPrice,
                UserId = uid
            };
            _db.Orders.Add(order);
            await _db.SaveChangesAsync();
            return CreatedAtAction(nameof(GetOrder), new { id = order.Id }, order);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateOrder(int id, [FromBody] OrderUpdateDto dto)
        {
            var uid = CurrentUserId;
            var order = await _db.Orders.FirstOrDefaultAsync(o => o.Id == id && o.UserId == uid);
            if (order == null) return NotFound();

            order.ProductName = dto.ProductName;
            order.Quantity = dto.Quantity;
            order.UnitPrice = dto.UnitPrice;
            order.TotalAmount = dto.Quantity * dto.UnitPrice;

            await _db.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            var uid = CurrentUserId;
            var order = await _db.Orders.FirstOrDefaultAsync(o => o.Id == id && o.UserId == uid);
            if (order == null) return NotFound();
            _db.Orders.Remove(order);
            await _db.SaveChangesAsync();
            return NoContent();
        }
    }

    public class OrderCreateDto { public string ProductName { get; set; } public int Quantity { get; set; } public decimal UnitPrice { get; set; } }
    public class OrderUpdateDto { public string ProductName { get; set; } public int Quantity { get; set; } public decimal UnitPrice { get; set; } }
}
