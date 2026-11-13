using DessertOrders.Data;
using DessertOrders.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System;

namespace DessertOrders.Pages.Admin
{
    public class CatalogModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public CatalogModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<DessertViewModel> Desserts { get; set; } = new List<DessertViewModel>();

        // Статистика для карточек сверху
        public int TotalCount { get; set; }
        public int ActiveCount { get; set; }
        public int OutOfStockCount { get; set; }
        public int DiscontinuedCount { get; set; }

        public async Task OnGetAsync()
        {
            var items = await _context.Desserts.ToListAsync();

            TotalCount = items.Count;
            ActiveCount = items.Count(d => d.IsAvailable && d.Stock > 0);
            OutOfStockCount = items.Count(d => d.IsAvailable && d.Stock == 0);
            DiscontinuedCount = items.Count(d => !d.IsAvailable);

            Desserts = items.Select(d => new DessertViewModel
            {
                Id = d.Id,
                Name = d.Name,
                Category = d.Category,
                Price = d.Price,
                Weight = d.Weight,
                Stock = d.Stock,
                ImageUrl = string.IsNullOrEmpty(d.ImageUrl) ? "/images/default-dessert.jpg" : d.ImageUrl,
                Status = !d.IsAvailable ? "Discontinued" : (d.Stock > 0 ? "Active" : "OutOfStock")
            }).ToList();
        }

        public async Task<IActionResult> OnPostUpdateStatusAsync(int id, string status)
        {
            var dessert = await _context.Desserts.FindAsync(id);
            if (dessert == null)
            {
                return new JsonResult(new { success = false, message = "Десерт не знайдено" });
            }

            if (status == "Discontinued")
            {
                dessert.IsAvailable = false;
            }
            else if (status == "Active")
            {
                dessert.IsAvailable = true;
            }

            await _context.SaveChangesAsync();
            return new JsonResult(new { success = true });
        }

        public async Task<IActionResult> OnPostUpdateDessertAsync([FromBody] EditDessertDto dto)
        {
            var dessert = await _context.Desserts.FindAsync(dto.Id);
            if (dessert == null)
            {
                return new JsonResult(new { success = false, message = "Десерт не знайдено" });
            }

            dessert.Name = dto.Name;
            dessert.Category = dto.Category;
            dessert.Price = dto.Price;
            dessert.Stock = dto.Stock;
            dessert.Weight = dto.Weight;
            dessert.ImageUrl = dto.ImageUrl;

            await _context.SaveChangesAsync();
            return new JsonResult(new { success = true });
        }
    }

    public class DessertViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int Weight { get; set; }
        public int Stock { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
    }

    public class EditDessertDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public int Weight { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
    }
}