namespace DessertOrders.Models
{
    public class Dessert
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;       // Название десерта
        public string Category { get; set; } = string.Empty;    // Категория (Тістечка, Торти, Еклери и т.д.)
        public decimal Price { get; set; }                      // Цена в $ (или ₴)
        public int Weight { get; set; }                         // Вес в граммах
        public int Stock { get; set; }                          // Остаток (залишок, шт.)
        public string Description { get; set; } = string.Empty; // Описание
        public string Ingredients { get; set; } = string.Empty; // Ингредиенты
        public string ImageUrl { get; set; } = string.Empty;    // Ссылка на фото
        public bool IsAvailable { get; set; } = true;           // Доступен для заказа (публикация)
    }
}