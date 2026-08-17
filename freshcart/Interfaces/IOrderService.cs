using freshcart.DTOs;

namespace freshcart.Interfaces
{
    public interface IOrderService
    {
        Task<OrderDto> CreateOrderAsync(int userId,CreateOrderDto dto);
        Task<List<OrderDto>> GetMyOrdersAsync(int userId);
        Task<OrderDto?> GetOrderByIdAsync(int orderId,int userId);
    }
}
