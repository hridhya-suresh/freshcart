using freshcart.Interfaces;
using Microsoft.EntityFrameworkCore;
using Razorpay.Api;

namespace freshcart.Services

{
    public class RazorpayService : IRazorpayService
    {
        private readonly string _keyId;
        private readonly string _keySecret;
        private readonly string _webhookSecret;

        public RazorpayService(IConfiguration configuration)
        {
            _keyId = configuration["Razorpay:KeyId"]
                ?? throw new Exception("Razorpay KeyId is missing.");

            _keySecret = configuration["Razorpay:KeySecret"]
                ?? throw new Exception("Razorpay KeySecret is missing.");

            _webhookSecret = configuration["Razorpay:WebhookSecret"]
                ?? throw new Exception("Razorpay WebhookSecret is missing.");
        }
        public async Task<string> CreateOrderAsync(decimal amount,string receipt)
        {
            var client = new RazorpayClient(_keyId,_keySecret );
            var options = new Dictionary<string, object>
            {
                {
                    "amount",
                    Convert.ToInt64(amount * 100)
                },
                {
                    "currency",
                    "INR"
                },
                {
                    "receipt",
                    receipt
                }
            };

            var order = client.Order.Create(options);

            return order["id"].ToString()!;
        }

        public bool VerifyPaymentSignature(string razorpayOrderId,string razorpayPaymentId, string razorpaySignature)
        {
            var attributes = new Dictionary<string, string>
                {
                    {
                        "razorpay_order_id",
                        razorpayOrderId
                    },
                    {
                        "razorpay_payment_id",
                        razorpayPaymentId
                    },
                    {
                        "razorpay_signature",
                        razorpaySignature
                    }
                };

            try
            {
                Utils.verifyPaymentSignature(attributes);

                return true;
            }
            catch
            {
                return false;
            }
        }
        public bool VerifyWebhookSignature( string payload,  string signature)
        {
            try
            {
                Utils.verifyWebhookSignature(
                    payload,
                    signature,
                    _webhookSecret
                );

                return true;
            }
            catch
            {
                return false;
            }
        }
       
    }
}
