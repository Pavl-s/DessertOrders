using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using DessertOrders.Data;
using DessertOrders.Models;

namespace DessertOrders.Pages
{
    [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
    public class ProfileModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public ProfileModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public ProfileInput Input { get; set; } = new();

        public string UserInitials { get; set; } = "Г";
        public List<Order> UserOrders { get; set; } = new();
        public Order? LatestOrder { get; set; }

        // Модель введення для редагування персональних даних
        public class ProfileInput
        {
            public string Name { get; set; } = string.Empty;
            public string Phone { get; set; } = string.Empty;
            public string Email { get; set; } = string.Empty;
        }

        // Завантаження профілю та історії замовлень
        public async Task<IActionResult> OnGetAsync(int? id)
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToPage("/Login");
            }

            var user = await _context.Users.FindAsync(userId.Value);
            if (user == null)
            {
                return RedirectToPage("/Login");
            }

            Input = new ProfileInput
            {
                Name = user.Name,
                Phone = user.Phone ?? string.Empty,
                Email = user.Email
            };

            UserInitials = GetInitials(user.Name);

            // Завантаження всіх замовлень поточного користувача
            UserOrders = await _context.Orders
                .Include(o => o.Items)
                .Where(o => o.UserId == userId.Value)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();

            // Вибір конкретного замовлення за ID з параметрів URL або взяття останнього створеного
            if (id.HasValue)
            {
                LatestOrder = UserOrders.FirstOrDefault(o => o.Id == id.Value);
            }

            LatestOrder ??= UserOrders.FirstOrDefault();

            return Page();
        }

        // Оновлення персональних даних користувача
        public async Task<IActionResult> OnPostAsync()
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToPage("/Login");
            }

            var user = await _context.Users.FindAsync(userId.Value);
            if (user == null)
            {
                return NotFound();
            }

            user.Name = Input.Name;
            user.Phone = Input.Phone;
            user.Email = Input.Email;

            await _context.SaveChangesAsync();

            // Оновлюємо ім'я в сесії, щоб у шапці воно змінилося відразу
            HttpContext.Session.SetString("UserName", user.Name);

            TempData["SuccessMessage"] = "Профіль успішно оновлено.";
            return RedirectToPage();
        }

        // Хендлер виходу з акаунта
        public IActionResult OnGetLogout()
        {
            HttpContext.Session.Remove("UserId");
            HttpContext.Session.Clear();
            return RedirectToPage("/Catalog");
        }

        // Генерація ініціалів користувача для аватара
        private string GetInitials(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return "У";
            }

            var parts = name.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length >= 2)
            {
                return $"{char.ToUpper(parts[0][0])}{char.ToUpper(parts[1][0])}";
            }

            return char.ToUpper(parts[0][0]).ToString();
        }
    }
}