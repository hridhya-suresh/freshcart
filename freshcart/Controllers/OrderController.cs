using freshcart.DTOs;
using freshcart.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace freshcart.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class OrderController : Controller
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        private int GetUserId()
        {
            return int.Parse(
                User.FindFirst(
                    ClaimTypes.NameIdentifier
                )!.Value
            );
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrder(CreateOrderDto dto)
        {
            try
            {
                var order = await _orderService.CreateOrderAsync(GetUserId(), dto);

                return Ok(order);
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetMyOrders()
        {
            var orders =
                await _orderService.GetMyOrdersAsync(
                    GetUserId());

            return Ok(orders);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrder(int id)
        {
            var order =
                await _orderService.GetOrderByIdAsync(
                    id,
                    GetUserId());

            if (order == null)
                return NotFound();

            return Ok(order);
        }

    }
}
