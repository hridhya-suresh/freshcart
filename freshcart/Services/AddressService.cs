using freshcart.Data;
using freshcart.DTOs;
using freshcart.Interfaces;
using freshcart.Models;
using Microsoft.EntityFrameworkCore;

namespace freshcart.Services
{
    public class AddressService : IAddressService
    {
        private readonly ApplicationDbContext _context;

        public AddressService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<AddressDto>> GetAddressesAsync(int userId)
        {
            return await _context.Addresses

                .Where(x => x.UserId == userId)

                .Select(x => new AddressDto
                {
                    AddressId = x.AddressId,
                    FullName = x.FullName,
                    Phone = x.Phone,
                    AddressLine1 = x.AddressLine1,
                    AddressLine2 = x.AddressLine2,
                    Landmark = x.Landmark,
                    City = x.City,
                    State = x.State,
                    PostalCode = x.PostalCode,
                    Country = x.Country,
                    IsDefault = x.IsDefault
                })

                .ToListAsync();
        }
        public async Task<AddressDto> CreateAddressAsync(int userId,CreateAddressDto dto)
        {
            // If this is the user's first address,
            // make it the default address.
            bool isFirstAddress = !await _context.Addresses
                .AnyAsync(a => a.UserId == userId);

            var address = new Address
            {
                UserId = userId,
                FullName = dto.FullName,
                Phone = dto.Phone,
                AddressLine1 = dto.AddressLine1,
                AddressLine2 = dto.AddressLine2,
                Landmark = dto.Landmark,
                City = dto.City,
                State = dto.State,
                PostalCode = dto.PostalCode,
                Country = dto.Country,
                IsDefault = isFirstAddress,
                CreatedDate = DateTime.Now
            };

            _context.Addresses.Add(address);

            await _context.SaveChangesAsync();

            return new AddressDto
            {
                AddressId = address.AddressId,
                FullName = address.FullName,
                Phone = address.Phone,
                AddressLine1 = address.AddressLine1,
                AddressLine2 = address.AddressLine2,
                Landmark = address.Landmark,
                City = address.City,
                State = address.State,
                PostalCode = address.PostalCode,
                Country = address.Country,
                IsDefault = address.IsDefault
            };
        }
        public async Task<AddressDto?> UpdateAddressAsync(int addressId,int userId,CreateAddressDto dto)
        {
            var address = await _context.Addresses
                .FirstOrDefaultAsync(a =>
                    a.AddressId == addressId &&
                    a.UserId == userId);

            if (address == null)
                return null;

            address.FullName = dto.FullName;
            address.Phone = dto.Phone;
            address.AddressLine1 = dto.AddressLine1;
            address.AddressLine2 = dto.AddressLine2;
            address.Landmark = dto.Landmark;
            address.City = dto.City;
            address.State = dto.State;
            address.PostalCode = dto.PostalCode;
            address.Country = dto.Country;

            await _context.SaveChangesAsync();

            return new AddressDto
            {
                AddressId = address.AddressId,
                FullName = address.FullName,
                Phone = address.Phone,
                AddressLine1 = address.AddressLine1,
                AddressLine2 = address.AddressLine2,
                Landmark = address.Landmark,
                City = address.City,
                State = address.State,
                PostalCode = address.PostalCode,
                Country = address.Country,
                IsDefault = address.IsDefault
            };
        }
        public async Task<bool> DeleteAddressAsync(int addressId,int userId)
        {
            var address = await _context.Addresses
                .FirstOrDefaultAsync(a =>
                    a.AddressId == addressId &&
                    a.UserId == userId);

            if (address == null)
                return false;

            _context.Addresses.Remove(address);

            await _context.SaveChangesAsync();

            return true;
        }
        public async Task<bool> SetDefaultAddressAsync(int addressId,int userId)
        {
            var address = await _context.Addresses
                .FirstOrDefaultAsync(a =>
                    a.AddressId == addressId &&
                    a.UserId == userId);

            if (address == null)
                return false;

            var existingDefaultAddresses = await _context.Addresses
                .Where(a =>
                    a.UserId == userId &&
                    a.IsDefault)
                .ToListAsync();

            foreach (var item in existingDefaultAddresses)
            {
                item.IsDefault = false;
            }

            address.IsDefault = true;

            await _context.SaveChangesAsync();

            return true;
        }
        public async Task<AddressDto?> GetDefaultAddressAsync(int userId)
        {
            return await _context.Addresses
                .Where(a =>
                    a.UserId == userId &&
                    a.IsDefault)
                .Select(a => new AddressDto
                {
                    AddressId = a.AddressId,
                    FullName = a.FullName,
                    Phone = a.Phone,
                    AddressLine1 = a.AddressLine1,
                    AddressLine2 = a.AddressLine2,
                    Landmark = a.Landmark,
                    City = a.City,
                    State = a.State,
                    PostalCode = a.PostalCode,
                    Country = a.Country,
                    IsDefault = a.IsDefault
                })
                .FirstOrDefaultAsync();
        }
    }
}
