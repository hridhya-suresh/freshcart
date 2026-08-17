using freshcart.DTOs;

namespace freshcart.Interfaces
{
    public interface IAddressService
    {
        Task<List<AddressDto>> GetAddressesAsync(int userId);

        Task<AddressDto?> GetDefaultAddressAsync(int userId);

        Task<AddressDto> CreateAddressAsync(int userId, CreateAddressDto dto);
        Task<AddressDto?> UpdateAddressAsync(int addressId,int userId, CreateAddressDto dto);
        Task<bool> DeleteAddressAsync(int addressId, int userId);
        Task<bool> SetDefaultAddressAsync(int addressId,int userId);

    }
}
