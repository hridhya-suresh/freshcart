using freshcart.Data;
using freshcart.Interfaces;
using freshcart.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace freshcart.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet]
        public async Task<IActionResult> GetProducts()
        {
            return Ok(await _productService.GetProductsAsync());
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetProduct(int id)
        {
            var product = await _productService.GetProductAsync(id);

            if (product == null)
                return NotFound();

            return Ok(product);
        }

        [HttpGet("{id}/related")]
        public async Task<IActionResult> GetRelatedProducts(int id)
        {
            return Ok(await _productService.GetRelatedProductsAsync(id));
        }
    }
}
