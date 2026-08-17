using freshcart.DTOs;
using freshcart.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace freshcart.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class AddressController : ControllerBase
    {
        private readonly IAddressService _addressService;

        public AddressController(
            IAddressService addressService)
        {
            _addressService = addressService;
        }

        private int GetUserId()
        {
            return int.Parse(

                User.FindFirst(ClaimTypes.NameIdentifier)!.Value

            );
        }
        [HttpGet]
        public async Task<IActionResult> GetAddresses()
        {
            var addresses =
                await _addressService.GetAddressesAsync(GetUserId());

            return Ok(addresses);
        }

        [HttpPost]
        public async Task<IActionResult> CreateAddress(CreateAddressDto dto)
        {
            var address = await _addressService.CreateAddressAsync(GetUserId(), dto);

            return Ok(address);
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAddress(int id,CreateAddressDto dto)
        {
            var result = await _addressService.UpdateAddressAsync(id,GetUserId(),dto);

            if (result == null)
                return NotFound();

            return Ok(result);
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAddress(int id)
        {
            await _addressService.DeleteAddressAsync(id, GetUserId());
            return NoContent();
        }
        [HttpPut("{id}/default")]
        public async Task<IActionResult> SetDefaultAddress(int id)
        {
            var result = await _addressService.SetDefaultAddressAsync(id,GetUserId());

            if (!result)
                return NotFound();

            return Ok(new
            {
                message = "Default address updated successfully"
            });
        }
        [HttpGet("default")]
        public async Task<IActionResult> GetDefaultAddress()
        {
            var address = await _addressService
                .GetDefaultAddressAsync(GetUserId());

            if (address == null)
                return NotFound();

            return Ok(address);
        }
    }
}
