using freshcart.Data;
using freshcart.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace freshcart.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly ApplicationDbContext _context;
        private readonly IOrderRealtimeService _realtime;

        public PaymentService(
            ApplicationDbContext context,
            IOrderRealtimeService realtime)
        {
            _context = context;
            _realtime = realtime;
        }

        public async Task<bool> MarkOrderAsPaidAsync(int orderId, string razorpayPaymentId)
        {
            var order = await _context.Orders
                .FirstOrDefaultAsync(o => o.OrderId == orderId);

            if (order == null)
                return false;

            // Already paid - nothing more to do
            if (order.PaymentStatus == "Paid")
                return true;

            order.PaymentStatus = "Paid";
            order.OrderStatus = "Confirmed";
            order.RazorpayPaymentId = razorpayPaymentId;
            order.PaymentDate = DateTime.UtcNow;

            var cartItems = await _context.CartItems
                .Where(c => c.UserId == order.UserId)
                .ToListAsync();

            _context.CartItems.RemoveRange(cartItems);

            await _context.SaveChangesAsync();

            await _realtime.NotifyOrderStatusChangedAsync(
                order.OrderId,
                order.UserId,
                order.OrderStatus,
                order.PaymentStatus);

            return true;
        }

        public async Task<bool> MarkOrderAsFailedAsync(int orderId)
        {
            var order = await _context.Orders
                .FirstOrDefaultAsync(o => o.OrderId == orderId);

            if (order == null)
                return false;

            // Don't change a successfully paid order back to failed
            if (order.PaymentStatus == "Paid")
                return true;

            // Avoid noisy duplicate notifications
            if (order.PaymentStatus == "Failed")
                return true;

            order.PaymentStatus = "Failed";

            await _context.SaveChangesAsync();

            await _realtime.NotifyOrderStatusChangedAsync(
                order.OrderId,
                order.UserId,
                order.OrderStatus,
                order.PaymentStatus);

            return true;
        }
    }
}
