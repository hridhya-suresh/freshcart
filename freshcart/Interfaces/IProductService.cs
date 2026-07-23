using freshcart.Models;

namespace freshcart.Interfaces
{
    public interface IProductService
    {
        Task<List<Product>> GetProductsAsync();
        Task<Product?> GetProductAsync(int id);
        Task<List<Product>> GetRelatedProductsAsync(int id);
    }
}
