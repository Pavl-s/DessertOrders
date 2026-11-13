using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using DessertOrders.Data;
using DessertOrders.Models;

namespace DessertOrders.Pages.Admin
{
    public class AddDessertModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public AddDessertModel(ApplicationDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        [BindProperty]
        public Dessert Dessert { get; set; } = new();

        [BindProperty]
        public IFormFile? ImageFile { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return RedirectToPage("/Login");

            var user = await _context.Users.FindAsync(userId.Value);
            if (user == null || user.Role != "Admin") return RedirectToPage("/Catalog");

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            int? userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null) return RedirectToPage("/Login");

            if (ImageFile != null && ImageFile.Length > 0)
            {
                var uploadsFolder = Path.Combine(_environment.WebRootPath, "images", "desserts");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                var uniqueFileName = Guid.NewGuid().ToString() + "_" + ImageFile.FileName;
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await ImageFile.CopyToAsync(fileStream);
                }

                Dessert.ImageUrl = "/images/desserts/" + uniqueFileName;
            }
            else
            {
                Dessert.ImageUrl = "/images/placeholder.jpg";
            }

            _context.Desserts.Add(Dessert);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Десерт успішно додано до вітрини!";
            return RedirectToPage("/Admin/AddDessert");
        }
    }
}