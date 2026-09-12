namespace freshcart.DTOs.Payment
{
    public class CreatePaymentOrderRequest
    {
        public int OrderId { get; set; }

    }
    public class PaymentOrderResponse
    {
        public string RazorpayOrderId { get; set; } = "";

        public string KeyId { get; set; } = "";

        public decimal Amount { get; set; }

        public string Currency { get; set; } = "INR";

        public int OrderId { get; set; }
    }
}
