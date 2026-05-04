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

        public async Task<IActionResult> Index(ProductCategory? category, string? searchTerm)
        {
            var products = _context.Products.AsQueryable();

            if (category.HasValue)
            {
                products = products.Where(p => p.Category == category.Value);
            }

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                searchTerm = searchTerm.Trim();

                products = products.Where(p =>
                    p.Name.Contains(searchTerm) ||
                    (p.Description != null && p.Description.Contains(searchTerm)));
            }

            ViewBag.CurrentCategory = category?.ToString();
            ViewBag.SearchTerm = searchTerm;

            return View(await products
                .OrderBy(p => p.Name)
                .ToListAsync());
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
