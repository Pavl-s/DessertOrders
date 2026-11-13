namespace DessertOrders.Models
{
    public class Dessert
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;       
        public string Category { get; set; } = string.Empty;    
        public decimal Price { get; set; }                      
        public int Weight { get; set; }                         
        public int Stock { get; set; }                          
        public string Description { get; set; } = string.Empty; 
        public string Ingredients { get; set; } = string.Empty; 
        public string ImageUrl { get; set; } = string.Empty;    
        public bool IsAvailable { get; set; } = true;           
    }
}