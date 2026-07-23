using freshcart.Data;
using freshcart.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;

namespace freshcart.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class CartController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public CartController(ApplicationDbContext context)
    {
        _context = context;
    }
    [Authorize]
    [HttpPost]
    public async Task<IActionResult> AddToCart(AddToCartDto dto)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

        if (userIdClaim == null)
            return Unauthorized();

        int userId = int.Parse(userIdClaim.Value);

        // Check if product exists
        var product = await _context.Products
            .FirstOrDefaultAsync(x => x.ProductId == dto.ProductId);

        if (product == null)
            return NotFound("Product not found");

        // Check if already in cart
        var cartItem = await _context.CartItems
            .FirstOrDefaultAsync(x =>
                x.UserId == userId &&
                x.ProductId == dto.ProductId);

        if (cartItem != null)
        {
            cartItem.Quantity += dto.Quantity;
        }
        else
        {
            cartItem = new CartItem
            {
                UserId = userId,
                ProductId = dto.ProductId,
                Quantity = dto.Quantity,
                CreatedDate = DateTime.Now
            };

            _context.CartItems.Add(cartItem);
        }

        await _context.SaveChangesAsync();

        return Ok(new
        {
            Message = "Product added to cart successfully."
        });
    }

    [HttpGet]
    public async Task<IActionResult> GetCart()
    {
        var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);

        if (userIdClaim == null)
            return Unauthorized();

        int userId = int.Parse(userIdClaim.Value);

        var cart = await _context.CartItems

            .Where(c => c.UserId == userId)

            .Include(c => c.Product)

            .Select(c => new CartDto
            {
                CartItemId = c.CartItemId,

                ProductId = c.ProductId,

                ProductName = c.Product!.ProductName,

                Price = c.Product.Price,

                ImageUrl = c.Product.ImageUrl,

                Quantity = c.Quantity,

                Subtotal = c.Product.Price * c.Quantity
            })

            .ToListAsync();

        return Ok(cart);
    }
    [HttpPut("{cartItemId}")]
    public async Task<IActionResult> UpdateQuantity(int cartItemId, UpdateCartDto dto)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

        if (userIdClaim == null)
            return Unauthorized();

        int userId = int.Parse(userIdClaim.Value);

        var cartItem = await _context.CartItems
            .FirstOrDefaultAsync(c =>
                c.CartItemId == cartItemId &&
                c.UserId == userId);

        if (cartItem == null)
            return NotFound();

        if (dto.Quantity <= 0)
        {
            _context.CartItems.Remove(cartItem);
        }
        else
        {
            cartItem.Quantity = dto.Quantity;
        }

        await _context.SaveChangesAsync();

        return Ok(new
        {
            message = "Cart updated"
        });
    }
    [HttpDelete("{cartItemId}")]
    public async Task<IActionResult> DeleteCartItem(int cartItemId)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

        if (userIdClaim == null)
            return Unauthorized();

        int userId = int.Parse(userIdClaim.Value);

        var cartItem = await _context.CartItems
            .FirstOrDefaultAsync(c =>
                c.CartItemId == cartItemId &&
                c.UserId == userId);

        if (cartItem == null)
            return NotFound();

        _context.CartItems.Remove(cartItem);

        await _context.SaveChangesAsync();

        return Ok(new
        {
            message = "Item removed"
        });
    }
}