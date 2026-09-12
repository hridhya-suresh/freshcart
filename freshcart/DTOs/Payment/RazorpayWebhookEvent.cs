namespace freshcart.DTOs.Payment
{
    public class RazorpayWebhookEvent
    {
        public string Event { get; set; } = "";

        public RazorpayWebhookPayload Payload { get; set; } = new();
    }

    public class RazorpayWebhookPayload
    {
        public RazorpayPaymentWrapper? Payment { get; set; }
    }

    public class RazorpayPaymentWrapper
    {
        public RazorpayPaymentEntity Entity { get; set; } = new();
    }

    public class RazorpayPaymentEntity
    {
        public string Id { get; set; } = "";

        public string Order_Id { get; set; } = "";

        public string Status { get; set; } = "";

        public long Amount { get; set; }
    }
}