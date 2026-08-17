using System.ComponentModel.DataAnnotations;

namespace freshcart.DTOs
{
    public class AddressDto
    {
        public int AddressId { get; set; }

        public string FullName { get; set; } = "";

        public string Phone { get; set; } = "";

        public string AddressLine1 { get; set; } = "";

        public string? AddressLine2 { get; set; }

        public string? Landmark { get; set; }

        public string City { get; set; } = "";

        public string State { get; set; } = "";

        public string PostalCode { get; set; } = "";

        public string Country { get; set; } = "";
        public bool IsDefault { get; set; }

    }
    public class CreateAddressDto
    {
        [Required]
        public string FullName { get; set; } = "";

        [Required]
        public string Phone { get; set; } = "";

        [Required]
        public string AddressLine1 { get; set; } = "";

        public string? AddressLine2 { get; set; }

        public string? Landmark { get; set; }

        [Required]
        public string City { get; set; } = "";

        [Required]
        public string State { get; set; } = "";

        [Required]
        public string PostalCode { get; set; } = "";

        public string Country { get; set; } = "India";
    }
}
