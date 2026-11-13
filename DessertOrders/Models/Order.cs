using System;
using System.Collections.Generic;

namespace DessertOrders.Models
{
    public class Order
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public User? User { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public string DeliveryType { get; set; } = "Delivery";
        public string? Address { get; set; }
        public string? Apartment { get; set; }
        public string DeliveryTime { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string PaymentMethod { get; set; } = string.Empty;
        public decimal ItemsTotal { get; set; }
        public decimal DeliveryFee { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; } = "Новий";

        public List<OrderItem> Items { get; set; } = new();

    }
}
