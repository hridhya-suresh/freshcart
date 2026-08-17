namespace freshcart.DTOs
{
        public class OrderDto
        {
            public int OrderId { get; set; }

            public DateTime OrderDate { get; set; }

            public decimal TotalAmount { get; set; }

            public string OrderStatus { get; set; } = "";

            public string PaymentStatus { get; set; } = "";

            public string PaymentMethod { get; set; } = "";

            public int AddressId { get; set; }

            public List<OrderItemDto> Items { get; set; }
                = new();
            public OrderAddressDto? ShippingAddress { get; set; }
    }
    public class OrderItemDto
    {
        public int ProductId { get; set; }

        public string ProductName { get; set; } = "";

        public string? ImageUrl { get; set; }

        public int Quantity { get; set; }

        public decimal Price { get; set; }

        public decimal SubTotal { get; set; }
    }
    public class CreateOrderDto
        {
            public int AddressId { get; set; }

            public string PaymentMethod { get; set; } = "CashOnDelivery";
        }
    public class OrderAddressDto
    {
        public string FullName { get; set; } = "";

        public string Phone { get; set; } = "";

        public string AddressLine1 { get; set; } = "";

        public string? AddressLine2 { get; set; }

        public string? Landmark { get; set; }

        public string City { get; set; } = "";

        public string State { get; set; } = "";

        public string PostalCode { get; set; } = "";

        public string Country { get; set; } = "";
    }
}
