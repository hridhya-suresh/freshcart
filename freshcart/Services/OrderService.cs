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
        private readonly IOrderRealtimeService _realtime;

        public OrderService(ApplicationDbContext context, IOrderRealtimeService realtime)
        {
            _context = context;
            _realtime = realtime;
        }
        private async Task<Models.Order?> FindExistingUnpaidOrderAsync( int userId,int addressId, string paymentMethod, List<CartItem> cartItems)
        {
            if (paymentMethod != "Online")
                return null;

           var unpaidOrders = await _context.Orders
            .Where(o =>
                o.UserId == userId &&
                o.PaymentMethod == "Online" &&
                o.PaymentStatus != "Paid" &&
                o.OrderStatus != "Cancelled")
            .Include(o => o.OrderItems)
                .ThenInclude(i => i.Product)
            .ToListAsync();

            foreach (var order in unpaidOrders)
            {
                if (order.AddressId != addressId)
                    continue;

                if (order.OrderItems.Count != cartItems.Count)
                    continue;

                bool sameItems = order.OrderItems.All(orderItem =>
                    cartItems.Any(cartItem =>
                        cartItem.ProductId == orderItem.ProductId &&
                        cartItem.Quantity == orderItem.Quantity
                    )
                );

                if (sameItems)
                {
                    return order;
                }
            }

            return null;
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
            var existingOrder =
            await FindExistingUnpaidOrderAsync(
                userId,
                dto.AddressId,
                dto.PaymentMethod,
                cartItems);

            if (existingOrder != null)
            {
                return new OrderDto
                {
                    OrderId = existingOrder.OrderId,
                    OrderDate = existingOrder.OrderDate,
                    TotalAmount = existingOrder.TotalAmount,
                    OrderStatus = existingOrder.OrderStatus,
                    PaymentStatus = existingOrder.PaymentStatus,
                    PaymentMethod = existingOrder.PaymentMethod,
                    AddressId = existingOrder.AddressId,

                    Items = existingOrder.OrderItems
                        .Select(item => new OrderItemDto
                        {
                            ProductId = item.ProductId,
                            ProductName =
                                item.Product?.ProductName ?? "",
                            ImageUrl =
                                item.Product?.ImageUrl,
                            Quantity = item.Quantity,
                            Price = item.Price,
                            SubTotal = item.SubTotal
                        })
                        .ToList()
                };
            }

            // Create order
            var order = new Models.Order
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

            // Clear cart before saving only for CashOnDelivery
            if (dto.PaymentMethod == "CashOnDelivery")
            {
                _context.CartItems.RemoveRange(cartItems);
            }

            // Save so order.OrderId is generated
            await _context.SaveChangesAsync();

            // Useful if the client already joined user-{userId} (e.g. on orders page)
            await _realtime.NotifyOrderStatusChangedAsync(
                order.OrderId,
                order.UserId,
                order.OrderStatus,
                order.PaymentStatus);

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
