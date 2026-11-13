using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using DessertOrders.Data;

namespace DessertOrders.Pages
{
    public class LoginModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public LoginModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public LoginInput Input { get; set; } = new();

        // Модель введення даних для форми авторизації
        public class LoginInput
        {
            [Required(ErrorMessage = "Введіть email")]
            [EmailAddress(ErrorMessage = "Невірний формат email")]
            public string Email { get; set; } = string.Empty;

            [Required(ErrorMessage = "Введіть пароль")]
            [DataType(DataType.Password)]
            public string Password { get; set; } = string.Empty;
        }

        public void OnGet()
        {
            // Відображення сторінки входу
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Пошук користувача за паролем та email
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == Input.Email && u.Password == Input.Password);

            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Невірний email або пароль");
                return Page();
            }

            // Збереження атрибутів авторизації в сесію
            HttpContext.Session.SetInt32("UserId", user.Id);
            HttpContext.Session.SetString("UserName", user.Name);
            HttpContext.Session.SetString("UserRole", user.Role);

            // Перенаправлення залежно від ролі користувача
            if (user.Role == "Admin")
            {
                return RedirectToPage("/Admin/Orders");
            }

            return RedirectToPage("/Catalog");
        }
    }
}