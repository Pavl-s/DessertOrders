using System.ComponentModel.DataAnnotations;

namespace DessertOrders.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;

        [Required]
        public string Phone { get; set; } = string.Empty;

        public string Role { get; set; } = "Client";

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public List<Order> Orders { get; set; } = new();
    }
}