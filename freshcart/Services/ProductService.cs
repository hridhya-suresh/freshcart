using freshcart.Interfaces;
using freshcart.Models;
using Microsoft.EntityFrameworkCore;
using freshcart.Data;

namespace freshcart.Services
{
    public class ProductService : IProductService
    {
        private readonly ApplicationDbContext _context;
        public ProductService(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<List<Product>> GetProductsAsync()
        {
            return await _context.Products
                .Include(p => p.Category)
                .ToListAsync();
        }
        public async Task<Product?> GetProductAsync(int id)
        {
            return await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.ProductId == id);
        }
        public async Task<List<Product>> GetRelatedProductsAsync(int id)
        {
            var product = await _context.Products
                .FirstOrDefaultAsync(p => p.ProductId == id);
            if (product == null)
                return new List<Product>();
            return await _context.Products
                .Include(p => p.Category)
                .Where(p => p.CategoryId == product.CategoryId && p.ProductId != id)
                .ToListAsync();
        }
    }
}
