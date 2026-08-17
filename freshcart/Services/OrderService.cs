using freshcart.Data;
using freshcart.DTOs;
using freshcart.Interfaces;
using freshcart.Models;
using Microsoft.EntityFrameworkCore;

namespace freshcart.Services
{
    public class OrderService : IOrderService
    {
        private readonly ApplicationDbContext _context;

        public OrderService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<OrderDto> CreateOrderAsync(int userId, CreateOrderDto dto)
        {
            // Get address belonging to logged-in user
            var address = await _context.Addresses
                .FirstOrDefaultAsync(a =>
                    a.AddressId == dto.AddressId &&
                    a.UserId == userId);

            if (address == null)
                throw new Exception("Invalid address.");

            // Get cart items
            var cartItems = await _context.CartItems
                .Include(c => c.Product)
                .Where(c => c.UserId == userId)
                .ToListAsync();

            if (!cartItems.Any())
                throw new Exception("Cart is empty.");

            // Create order
            var order = new Order
            {
                UserId = userId,
                AddressId = dto.AddressId,
                OrderDate = DateTime.Now,
                OrderStatus = "Pending",
                PaymentStatus = "Pending",
                PaymentMethod = dto.PaymentMethod
            };

            decimal totalAmount = 0;

            foreach (var cartItem in cartItems)
            {
                if (cartItem.Product == null)
                    continue;

                decimal subtotal =
                    cartItem.Product.Price *
                    cartItem.Quantity;

                var orderItem = new OrderItem
                {
                    ProductId = cartItem.ProductId,
                    Quantity = cartItem.Quantity,
                    Price = cartItem.Product.Price,
                    SubTotal = subtotal
                };

                order.OrderItems.Add(orderItem);

                totalAmount += subtotal;
            }

            order.TotalAmount = totalAmount;

            _context.Orders.Add(order);

            // Clear cart after creating order
            _context.CartItems.RemoveRange(cartItems);

            await _context.SaveChangesAsync();

            return new OrderDto
            {
                OrderId = order.OrderId,
                OrderDate = order.OrderDate,
                TotalAmount = order.TotalAmount,
                OrderStatus = order.OrderStatus,
                PaymentStatus = order.PaymentStatus,
                PaymentMethod = order.PaymentMethod,
                AddressId = order.AddressId,

                Items = order.OrderItems
                    .Select(item => new OrderItemDto
                    {
                        ProductId = item.ProductId,
                        ProductName = item.Product?.ProductName ?? "",
                        ImageUrl = item.Product?.ImageUrl,
                        Quantity = item.Quantity,
                        Price = item.Price,
                        SubTotal = item.SubTotal
                    })
                    .ToList()
            };
        }

        public async Task<List<OrderDto>> GetMyOrdersAsync(int userId)
        {
            return await _context.Orders
                .Where(o => o.UserId == userId)
                .Include(o => o.OrderItems)
                    .ThenInclude(i => i.Product)
                .OrderByDescending(o => o.OrderDate)
                .Select(o => new OrderDto
                {
                    OrderId = o.OrderId,
                    OrderDate = o.OrderDate,
                    TotalAmount = o.TotalAmount,
                    OrderStatus = o.OrderStatus,
                    PaymentStatus = o.PaymentStatus,
                    PaymentMethod = o.PaymentMethod,
                    AddressId = o.AddressId,

                    Items = o.OrderItems
                        .Select(i => new OrderItemDto
                        {
                            ProductId = i.ProductId,
                            ProductName =
                                i.Product!.ProductName,
                            ImageUrl =
                                i.Product.ImageUrl,
                            Quantity = i.Quantity,
                            Price = i.Price,
                            SubTotal = i.SubTotal
                        })
                        .ToList()
                })
                .ToListAsync();
        }

        public async Task<OrderDto?> GetOrderByIdAsync(int orderId,int userId)
        {
            return await _context.Orders
                .Where(o =>
                    o.OrderId == orderId &&
                    o.UserId == userId)
                .Include(o => o.Address)
                .Include(o => o.OrderItems)
                    .ThenInclude(i => i.Product)
                .Select(o => new OrderDto
                {
                    OrderId = o.OrderId,
                    OrderDate = o.OrderDate,
                    TotalAmount = o.TotalAmount,
                    OrderStatus = o.OrderStatus,
                    PaymentStatus = o.PaymentStatus,
                    PaymentMethod = o.PaymentMethod,
                    AddressId = o.AddressId,
                    ShippingAddress = new OrderAddressDto
                    {
                        FullName = o.Address!.FullName,
                        Phone = o.Address.Phone,
                        AddressLine1 = o.Address.AddressLine1,
                        AddressLine2 = o.Address.AddressLine2,
                        Landmark = o.Address.Landmark,
                        City = o.Address.City,
                        State = o.Address.State,
                        PostalCode = o.Address.PostalCode,
                        Country = o.Address.Country
                    },

                    Items = o.OrderItems
                        .Select(i => new OrderItemDto
                        {
                            ProductId = i.ProductId,
                            ProductName =
                                i.Product!.ProductName,
                            ImageUrl =
                                i.Product.ImageUrl,
                            Quantity = i.Quantity,
                            Price = i.Price,
                            SubTotal = i.SubTotal
                        })
                        .ToList()
                })
                .FirstOrDefaultAsync();
        }
    
}
}
