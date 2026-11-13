using DessertOrders.Data;
using DessertOrders.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace DessertOrders.Pages.Admin
{
    public class ClientsModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public ClientsModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<ClientViewModel> Clients { get; set; } = new();
        public ClientViewModel? SelectedClient { get; set; }

        public int TotalClientsCount { get; set; }
        public int ActiveThisMonthCount { get; set; }
        public int NewTodayCount { get; set; }

        public async Task OnGetAsync(int? selectedUserId)
        {
            var users = await _context.Users
                .Include(u => u.Orders)
                .ThenInclude(o => o.Items)
                .ToListAsync();

            TotalClientsCount = users.Count;

            var now = DateTime.UtcNow;
            ActiveThisMonthCount = users.Count(u => u.Orders.Any(o => o.CreatedAt.Month == now.Month && o.CreatedAt.Year == now.Year));
            NewTodayCount = users.Count(u => u.Orders.Any(o => o.CreatedAt.Date == now.Date));

            Clients = users.Select(u => new ClientViewModel
            {
                Id = u.Id,
                Name = u.Name,
                Email = u.Email,
                Phone = u.Phone ?? "Не вказано",
                CreatedAt = u.Orders.Any() ? u.Orders.Min(o => o.CreatedAt) : now,
                OrdersCount = u.Orders.Count,
                TotalSpent = u.Orders.Sum(o => o.TotalAmount),
                AverageCheck = u.Orders.Count > 0 ? Math.Round(u.Orders.Sum(o => o.TotalAmount) / u.Orders.Count, 2) : 0,
                Orders = u.Orders.OrderByDescending(o => o.CreatedAt).ToList()
            }).OrderByDescending(c => c.TotalSpent).ToList();

            if (selectedUserId.HasValue)
            {
                SelectedClient = Clients.FirstOrDefault(c => c.Id == selectedUserId.Value);
            }

            if (SelectedClient == null && Clients.Count > 0)
            {
                SelectedClient = Clients.First();
            }
        }
    }

    public class ClientViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public int OrdersCount { get; set; }
        public decimal TotalSpent { get; set; }
        public decimal AverageCheck { get; set; }
        public List<Order> Orders { get; set; } = new();

        public string Initials
        {
            get
            {
                if (string.IsNullOrEmpty(Name)) return "Г";
                var parts = Name.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length >= 2)
                    return $"{char.ToUpper(parts[0][0])}{char.ToUpper(parts[1][0])}";
                return $"{char.ToUpper(Name[0])}";
            }
        }
    }
}