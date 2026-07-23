using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace freshcart.Models;

public class CartItem
{
    [Key]
    public int CartItemId { get; set; }

    public int UserId { get; set; }

    public int ProductId { get; set; }

    public int Quantity { get; set; }

    public DateTime CreatedDate { get; set; }

    // Navigation Properties

    [ForeignKey(nameof(UserId))]
    public User? User { get; set; }

    [ForeignKey(nameof(ProductId))]
    public Product? Product { get; set; }
}

public class AddToCartDto
{
    public int ProductId { get; set; }

    public int Quantity { get; set; } = 1;
}
public class CartDto
{
    public int CartItemId { get; set; }

    public int ProductId { get; set; }

    public string ProductName { get; set; } = "";

    public decimal Price { get; set; }

    public string ImageUrl { get; set; } = "";

    public int Quantity { get; set; }

    public decimal Subtotal { get; set; }
}

public class UpdateCartDto
{
    public int Quantity { get; set; }
}