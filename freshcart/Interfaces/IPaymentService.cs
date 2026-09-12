namespace freshcart.Interfaces
{
    public interface IPaymentService
    {
        Task<bool> MarkOrderAsPaidAsync(int orderId, string razorpayPaymentId);
        Task<bool> MarkOrderAsFailedAsync(int orderId);
    }
}
