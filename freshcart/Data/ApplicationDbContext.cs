using freshcart.Models;
using Microsoft.EntityFrameworkCore;

namespace freshcart.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {          
        }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<CartItem> CartItems { get; set; }

    }
}
