using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using freshcart.Data;
namespace freshcart.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class CategoriesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CategoriesController(
            ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetCategories()
        {
            return Ok(await _context.Categories.ToListAsync());
        }
    }
}
