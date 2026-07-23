namespace freshcart.Models
{
    public class User
    {
            public int UserId { get; set; }

            public string FirstName { get; set; } = string.Empty;

            public string LastName { get; set; } = string.Empty;

            public string Email { get; set; } = string.Empty;

            public string PasswordHash { get; set; } = string.Empty;

            public DateTime CreatedDate { get; set; }

            public bool IsActive { get; set; }
    }
    public class RegisterDto
    {
        public string FirstName { get; set; } = string.Empty;

        public string LastName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;
    }
    public class LoginDto
    {
        public string Email { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;
    }
}
