namespace freshcart.DTOs.Payment
{
    public class VerifyPaymentRequest
    {
        public int OrderId { get; set; }

        public string RazorpayOrderId { get; set; } = "";

        public string RazorpayPaymentId { get; set; } = "";

        public string RazorpaySignature { get; set; } = "";
    }
}
