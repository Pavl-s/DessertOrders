using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using DessertOrders.Data;
using DessertOrders.Models;

namespace DessertOrders.Pages.Admin
{
    public class OrdersModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public OrdersModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<Order> AllOrders { get; set; } = new();
        public List<Order> DisplayOrders { get; set; } = new();
        public Order? SelectedOrder { get; set; }

        public string CurrentStatus { get; set; } = "Усі";

        [BindProperty(SupportsGet = true)]
        public string? SearchQuery { get; set; }

        // --- НОВІ ПОКАЗНИКИ ДЛЯ СТАТИСТИКИ ---
        public int OrdersTodayCount { get; set; }
        public int PendingOrdersCount { get; set; }
        public decimal DailyRevenue { get; set; }
        public int NewClientsTodayCount { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id, string? status)
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return RedirectToPage("/Login");

            var user = await _context.Users.FindAsync(userId.Value);
            if (user == null || user.Role != "Admin")
            {
                return RedirectToPage("/Catalog");
            }

            var today = DateTime.Today; // Початок сьогоднішнього дня (00:00)

            // 1. Загружаем ВСЕ заказы для неизменной статистики
            AllOrders = await _context.Orders
                .Include(o => o.User)
                .Include(o => o.Items)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();

            // --- ОБЧИСЛЕННЯ СТАТИСТИКИ ЗА СЬОГОДНІ ---
            // Замовлень сьогодні
            OrdersTodayCount = AllOrders.Count(o => o.CreatedAt >= today);

            // Чекають підтвердження (статус "New" або "Очікує")
            PendingOrdersCount = AllOrders.Count(o => o.Status == "New" || o.Status == "Очікує");

            // Виторг за день (сума TotalAmount замовлень, створених сьогодні)
            DailyRevenue = AllOrders
                .Where(o => o.CreatedAt >= today)
                .Sum(o => o.TotalAmount);

            // Нових клієнтів за сьогодні 
            // (Якщо у вас в моделі User є поле CreatedAt, наприклад: u.CreatedAt >= today)
            // Якщо такого поля немає, можна рахувати за унікальними клієнтами, що зробили замовлення сьогодні вперше, або додати поле CreatedAt в User.
            NewClientsTodayCount = await _context.Users
                .CountAsync(u => u.CreatedAt >= today);

            if (!string.IsNullOrEmpty(status))
            {
                CurrentStatus = status;
            }

            // 2. Фильтрация для таблицы
            var query = AllOrders.AsQueryable();

            // По статусу
            if (CurrentStatus != "Усі")
            {
                query = query.Where(o => o.Status == CurrentStatus);
            }

            // По поисковому запросу (номер ID, имя клиента или телефон)
            if (!string.IsNullOrEmpty(SearchQuery))
            {
                var q = SearchQuery.Trim().ToLower();
                query = query.Where(o =>
                    o.Id.ToString().Contains(q) ||
                    (o.User != null && o.User.Name != null && o.User.Name.ToLower().Contains(q)) ||
                    (!string.IsNullOrEmpty(o.Phone) && o.Phone.Contains(q))
                );
            }

            DisplayOrders = query.ToList();

            // 3. Выбор заказа для правой панели
            if (id.HasValue)
            {
                SelectedOrder = AllOrders.FirstOrDefault(o => o.Id == id.Value);
            }

            SelectedOrder ??= DisplayOrders.FirstOrDefault() ?? AllOrders.FirstOrDefault();

            return Page();
        }

        public async Task<IActionResult> OnPostUpdateStatusAsync(int orderId, string newStatus, string? currentStatus, string? searchQuery)
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return RedirectToPage("/Login");

            var admin = await _context.Users.FindAsync(userId.Value);
            if (admin == null || admin.Role != "Admin") return Forbid();

            var order = await _context.Orders.FindAsync(orderId);
            if (order != null)
            {
                order.Status = newStatus;
                await _context.SaveChangesAsync();
            }

            return RedirectToPage(new { id = orderId, status = currentStatus, searchQuery });
        }
    }
}