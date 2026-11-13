using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using DessertOrders.Models;
using DessertOrders.Data;

namespace DessertOrders.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly ApplicationDbContext _context;

        public IndexModel(ILogger<IndexModel> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        [BindProperty]
        public User InputUser { get; set; } = new();

        public void OnGet()
        {
            // Сторінка реєстрації
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Перевірка на існування користувача з таким Email
            var existingUser = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == InputUser.Email);

            if (existingUser != null)
            {
                ModelState.AddModelError("InputUser.Email", "Користувач із такою електронною поштою вже зареєстрований.");
                return Page();
            }

            // Сохранение нового пользователя
            _context.Users.Add(InputUser);
            await _context.SaveChangesAsync();

            // Автоматична авторизація користувача після реєстрації
            HttpContext.Session.SetInt32("UserId", InputUser.Id);
            HttpContext.Session.SetString("UserRole", InputUser.Role);

            // Перенаправлення до каталогу
            return RedirectToPage("/Catalog");
        }
    }
}