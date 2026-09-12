using freshcart.Data;
using freshcart.DTOs.Payment;
using freshcart.Interfaces;
using freshcart.Models;
using freshcart.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Razorpay.Api;
using System.Security.Claims;

namespace freshcart.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PaymentsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IRazorpayService _razorpayService;
        private readonly IConfiguration _configuration;
        private readonly IPaymentService _paymentService;

        public PaymentsController(
            ApplicationDbContext context,
            IRazorpayService razorpayService,
            IConfiguration configuration,
            IPaymentService paymentService)
        {
            _context = context;
            _razorpayService = razorpayService;
            _configuration = configuration;
            _paymentService = paymentService;
        }

        private int GetUserId()
        {
            return int.Parse(
                User.FindFirst(
                    ClaimTypes.NameIdentifier
                )!.Value
            );
        }

        [HttpPost("create-order")]
        public async Task<IActionResult> CreatePaymentOrder([FromBody] CreatePaymentOrderRequest request)
        {
            try
            {
                var userId = GetUserId();

                var order = await _context.Orders
                    .FirstOrDefaultAsync(o =>
                        o.OrderId == request.OrderId &&
                        o.UserId == userId);

                if (order == null)
                {
                    return NotFound(new
                    {
                        message = "Order not found."
                    });
                }

                if (order.PaymentMethod != "Online")
                {
                    return BadRequest(new
                    {
                        message = "This order is not an online payment order."
                    });
                }

                if (order.PaymentStatus == "Paid")
                {
                    return BadRequest(new
                    {
                        message = "Order is already paid."
                    });
                }

                // Amount comes from database,
                // NOT from Angular.

                var amount = order.TotalAmount;

                var receipt =$"order_{order.OrderId}";

                var razorpayOrderId = await _razorpayService.CreateOrderAsync(amount, receipt);

                // Save Razorpay Order ID
                order.RazorpayOrderId = razorpayOrderId;

                await _context.SaveChangesAsync();

                var keyId = _configuration["Razorpay:KeyId"];

                return Ok(new PaymentOrderResponse
                {
                    RazorpayOrderId = razorpayOrderId,

                    KeyId = keyId!,

                    Amount = amount,

                    Currency ="INR",

                    OrderId = order.OrderId
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
        }

        [HttpPost("verify")]
        public async Task<IActionResult> VerifyPayment([FromBody] VerifyPaymentRequest request)
        {
            try
            {
                var userId = GetUserId();

                var order = await _context.Orders
                    .Include(o => o.OrderItems)
                    .FirstOrDefaultAsync(o =>
                        o.OrderId == request.OrderId &&
                        o.UserId == userId);

                if (order == null)
                {
                    return NotFound(new
                    {
                        message = "Order not found."
                    });
                }

                // Make sure the Razorpay order belongs
                // to our database order.
                if (string.IsNullOrEmpty(order.RazorpayOrderId) ||
                    order.RazorpayOrderId != request.RazorpayOrderId)
                {
                    return BadRequest(new
                    {
                        message = "Invalid Razorpay order."
                    });
                }

                if (order.PaymentStatus == "Paid")
                {
                    return BadRequest(new
                    {
                        message = "Order is already paid."
                    });
                }

                var isValid = _razorpayService.VerifyPaymentSignature(request.RazorpayOrderId, request.RazorpayPaymentId,request.RazorpaySignature);

                if (!isValid)
                {
                    return BadRequest(new
                    {
                        message = "Payment verification failed."
                    });
                }

                await _paymentService.MarkOrderAsPaidAsync(request.OrderId, request.RazorpayPaymentId);
                // Payment verified successfully

                //order.RazorpayPaymentId = request.RazorpayPaymentId;

                //order.PaymentStatus = "Paid";

                //order.PaymentDate = DateTime.UtcNow;

                //order.OrderStatus = "Confirmed";

                //// Clear the user's cart
                //var cartItems = await _context.CartItems
                //    .Where(c => c.UserId == userId)
                //    .ToListAsync();

                //_context.CartItems.RemoveRange(cartItems);

                //await _context.SaveChangesAsync();

                return Ok(new
                {
                    message = "Payment verified successfully.",
                    orderId = order.OrderId,
                    paymentStatus = order.PaymentStatus
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
        }

        [AllowAnonymous]
        [HttpPost("webhook")]
        public async Task<IActionResult> RazorpayWebhook()
        {
            try
            {
                using var reader = new StreamReader(Request.Body);

                var payload = await reader.ReadToEndAsync();

                var signature =
                    Request.Headers["X-Razorpay-Signature"]
                        .FirstOrDefault();

                if (string.IsNullOrEmpty(signature))
                {
                    return BadRequest();
                }

                var eventId =
                    Request.Headers["x-razorpay-event-id"]
                        .FirstOrDefault();
                if (string.IsNullOrEmpty(eventId))
                {
                    return BadRequest();
                }
                // Verify webhook BEFORE reading JSON
                var isValid =
                    _razorpayService.VerifyWebhookSignature(
                        payload,
                        signature
                    );

                if (!isValid)
                {
                    Console.WriteLine(
                        "❌ Invalid Razorpay webhook signature"
                    );

                    return Unauthorized();
                }

                Console.WriteLine(
                    "✅ Razorpay webhook signature verified"
                );

                var alreadyProcessed =
                    await _context.ProcessedWebhookEvents
                        .AnyAsync(x =>
                            x.EventId == eventId);

                                if (alreadyProcessed)
                                {
                                    Console.WriteLine(
                                        $"ℹ️ Duplicate webhook ignored: {eventId}"
                                    );

                                    return Ok();
                                }

                var webhookEvent =
                    System.Text.Json.JsonSerializer
                        .Deserialize<RazorpayWebhookEvent>(
                            payload
                        );

                if (webhookEvent == null)
                {
                    return BadRequest();
                }

                Console.WriteLine(
                    $"Razorpay Event: {webhookEvent.Event}"
                );

                // PAYMENT CAPTURED
                if (webhookEvent.Event == "payment.captured")
                {
                    var payment =
                        webhookEvent.Payload.Payment?.Entity;

                    if (payment == null)
                    {
                        return BadRequest();
                    }

                    var order =
                        await _context.Orders
                            .FirstOrDefaultAsync(
                                o =>
                                    o.RazorpayOrderId ==
                                    payment.Order_Id
                            );

                    if (order == null)
                    {
                        Console.WriteLine(
                            $"Order not found: {payment.Order_Id}"
                        );

                        return Ok();
                    }

                    var success =
                        await _paymentService.MarkOrderAsPaidAsync(
                            order.OrderId,
                            payment.Id
                        );

                    if (success)
                    {
                        Console.WriteLine(
                            $"✅ Order {order.OrderId} marked as PAID"
                        );
                    }

                    _context.ProcessedWebhookEvents.Add(
                        new ProcessedWebhookEvent
                        {
                            EventId = eventId,
                            EventType = webhookEvent.Event,
                            ProcessedAt = DateTime.UtcNow
                        }
                    );

                    await _context.SaveChangesAsync();
                }
                // PAYMENT FAILED
                else if (webhookEvent.Event == "payment.failed")
                {
                    var payment =
                        webhookEvent.Payload.Payment?.Entity;

                    if (payment != null)
                    {
                        var order =
                            await _context.Orders
                                .FirstOrDefaultAsync(
                                    o =>
                                        o.RazorpayOrderId ==
                                        payment.Order_Id
                                );

                        if (order != null)
                        {
                            var success =
                                await _paymentService.MarkOrderAsFailedAsync(
                                    order.OrderId
                                );

                            if (success)
                            {
                                Console.WriteLine(
                                    $"❌ Payment failed for Order {order.OrderId}"
                                );
                            }
                        }
                    }

                    _context.ProcessedWebhookEvents.Add(
                        new ProcessedWebhookEvent
                        {
                            EventId = eventId,
                            EventType = webhookEvent.Event,
                            ProcessedAt = DateTime.UtcNow
                        }
                    );

                    await _context.SaveChangesAsync();
                }

                return Ok();
            }
            catch (Exception ex)
            {
                Console.WriteLine(
                    $"Webhook error: {ex.Message}"
                );

                return StatusCode(500);
            }
        }
        [HttpPost("retry")]
        public async Task<IActionResult> RetryPayment([FromBody] RetryPaymentRequest request)
        {
            try
            {
                var userId = GetUserId();

                var order = await _context.Orders
                    .FirstOrDefaultAsync(o =>
                        o.OrderId == request.OrderId &&
                        o.UserId == userId);

                if (order == null)
                {
                    return NotFound(new
                    {
                        message = "Order not found."
                    });
                }

                if (order.PaymentMethod != "Online")
                {
                    return BadRequest(new
                    {
                        message = "This order is not an online payment order."
                    });
                }

                if (order.PaymentStatus == "Paid")
                {
                    return BadRequest(new
                    {
                        message = "Order is already paid."
                    });
                }

                var receipt = $"order_{order.OrderId}_retry";

                var razorpayOrderId =
                    await _razorpayService.CreateOrderAsync(
                        order.TotalAmount,
                        receipt
                    );

                order.RazorpayOrderId = razorpayOrderId;

                order.PaymentStatus = "Pending";

                await _context.SaveChangesAsync();

                return Ok(new PaymentOrderResponse
                {
                    RazorpayOrderId = razorpayOrderId,
                    KeyId = _configuration["Razorpay:KeyId"]!,
                    Amount = order.TotalAmount,
                    Currency = "INR",
                    OrderId = order.OrderId
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
        }
    }
}