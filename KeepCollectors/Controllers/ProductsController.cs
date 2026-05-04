using DataAccessLayer.Data;
using DataAccessLayer.DataModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static DataAccessLayer.DataModels.Product;

namespace KeepCollectors.Controllers
{
    public class ProductsController : Controller
    {
        private readonly CollectorsKeepDbContext _context;

        public ProductsController(CollectorsKeepDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(ProductCategory? category)
        {
            var products = _context.Products.AsQueryable();

            if (category.HasValue)
            {
                products = products.Where(p => p.Category == category.Value);
            }

            return View(await products.ToListAsync());
        }

        public async Task<IActionResult> Details(int id)
        {
            var product = await _context.Products
                .FirstOrDefaultAsync(p => p.ProductID == id);

            if (product == null)
                return NotFound();

            return View(product);
        }
    }
}
