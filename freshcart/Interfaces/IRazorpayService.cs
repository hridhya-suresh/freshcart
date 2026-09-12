namespace freshcart.Interfaces
{
    public interface IRazorpayService
    {
        Task<string> CreateOrderAsync(decimal amount,string receipt);
        bool VerifyPaymentSignature(string razorpayOrderId, string razorpayPaymentId,string razorpaySignature);
        bool VerifyWebhookSignature(string payload, string signature);
    }
}

