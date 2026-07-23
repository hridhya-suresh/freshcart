namespace freshcart.Models
{
    public class Product
    {
        public int ProductId { get; set; }

        public int CategoryId { get; set; }

        public string ProductName { get; set; } = string.Empty;

        public string? Description { get; set; }

        public decimal Price { get; set; }

        public int StockQty { get; set; }

        public string? ImageUrl { get; set; }

        public bool IsFeatured { get; set; }

        public DateTime CreatedDate { get; set; }

        public Category? Category { get; set; }
        //public int Stock { get; set; }

    }

    public class ProductDetailsDto
    {
        public int ProductId { get; set; }

        public string ProductName { get; set; } = "";

        public decimal Price { get; set; }

        public string? Description { get; set; }

        public string? ImageUrl { get; set; }

        public int CategoryId { get; set; }

        public string CategoryName { get; set; } = "";

        public int Stock { get; set; }
    }
}
