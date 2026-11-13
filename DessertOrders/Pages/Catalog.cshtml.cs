using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using DessertOrders.Data;
using DessertOrders.Models;

namespace DessertOrders.Pages
{
    [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
    public class CatalogModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public CatalogModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<Dessert> Desserts { get; set; } = new();
        public string UserName { get; set; } = "Гість";

        // Отримання списку активних десертів та даних користувача
        public async Task OnGetAsync()
        {
            Desserts = await _context.Desserts
                .Where(d => d.IsAvailable)
                .ToListAsync();

            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId != null)
            {
                var user = await _context.Users.FindAsync(userId.Value);
                if (user != null)
                {
                    UserName = user.Name;
                }
            }
        }

        // DTO для обробки даних замовлення з фронтенду
        public class CreateOrderDto
        {
            public string DeliveryType { get; set; } = "Delivery";
            public string? Address { get; set; }
            public string? Apartment { get; set; }
            public string DeliveryTime { get; set; } = string.Empty;
            public string Phone { get; set; } = string.Empty;
            public string PaymentMethod { get; set; } = string.Empty;
            public List<OrderItemDto> Items { get; set; } = new();
        }

        public class OrderItemDto
        {
            public int Id { get; set; }
            public string Name { get; set; } = string.Empty;
            public decimal Price { get; set; }
            public int Quantity { get; set; }
        }

        // Хендлер створення замовлення та оновлення складських залишків
        public async Task<IActionResult> OnPostCreateOrderAsync([FromBody] CreateOrderDto dto)
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return new JsonResult(new { success = false, message = "Будь ласка, увійдіть у систему" });
            }

            if (dto == null || dto.Items.Count == 0)
            {
                return BadRequest(new { success = false, message = "Кошик порожній" });
            }

            decimal itemsTotal = 0;
            var orderItems = new List<OrderItem>();

            foreach (var item in dto.Items)
            {
                var dessert = await _context.Desserts.FindAsync(item.Id);
                if (dessert != null)
                {
                    // Перевірка наявності десертів на складі
                    if (dessert.Stock < item.Quantity)
                    {
                        return new JsonResult(new { success = false, message = $"На жаль, десерту \"{dessert.Name}\" недостатньо на складі." });
                    }

                    // Списання кількості зі складу
                    dessert.Stock -= item.Quantity;
                }

                itemsTotal += item.Price * item.Quantity;
                orderItems.Add(new OrderItem
                {
                    DessertId = item.Id,
                    DessertName = item.Name,
                    Price = item.Price,
                    Quantity = item.Quantity
                });
            }

            decimal deliveryFee = dto.DeliveryType == "Delivery" ? 250 : 0;

            var order = new Order
            {
                UserId = userId.Value,
                CreatedAt = DateTime.UtcNow,
                DeliveryType = dto.DeliveryType,
                Address = dto.Address,
                Apartment = dto.Apartment,
                DeliveryTime = dto.DeliveryTime,
                Phone = dto.Phone,
                PaymentMethod = dto.PaymentMethod,
                ItemsTotal = itemsTotal,
                DeliveryFee = deliveryFee,
                TotalAmount = itemsTotal + deliveryFee,
                Status = "Новий",
                Items = orderItems
            };

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            return new JsonResult(new { success = true, orderId = order.Id });
        }
    }
}