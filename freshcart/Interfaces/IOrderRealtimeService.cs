namespace freshcart.Interfaces
{
    public interface IOrderRealtimeService
    {
        Task NotifyOrderStatusChangedAsync(
            int orderId,
            int userId,
            string orderStatus,
            string paymentStatus);
    }
}
