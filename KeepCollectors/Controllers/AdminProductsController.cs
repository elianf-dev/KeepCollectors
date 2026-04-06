using DataAccessLayer.Data;
using DataAccessLayer.DataModels;
using KeepCollectors.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace KeepCollectors.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminProductsController : Controller
    {

        private readonly CollectorsKeepDbContext _context;
        private readonly IWebHostEnvironment _env;

        public AdminProductsController(CollectorsKeepDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        public async Task<IActionResult> Index()
        {
            var products = await _context.Products
                .OrderBy(p => p.ProductID)
                .ToListAsync();

            return View(products);
        }

        public IActionResult Create()
        {
            return View(new AdminProductsEditViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AdminProductsEditViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var product = new Product
            {
                Name = model.Name,
                Description = model.Description,
                Price = model.Price,
                QuantityAvailable = model.QuantityAvailable,
                ImagePath = await ResolveImagePath(model)
            };

            _context.Products.Add(product);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var product = await _context.Products.FirstOrDefaultAsync(p => p.ProductID == id);
            if (product == null) return NotFound();

            var vm = new AdminProductsEditViewModel
            {
                ProductID = product.ProductID,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                QuantityAvailable = product.QuantityAvailable,
                ExistingImagePath = product.ImagePath
            };

            return View(vm); ;
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, AdminProductsEditViewModel model)
        {
            if (id != model.ProductID) return BadRequest();
            if (!ModelState.IsValid) return View(model);

            var product = await _context.Products.FirstOrDefaultAsync(p => p.ProductID == id);
            if (product == null) return NotFound();

            product.Name = model.Name;
            product.Description = model.Description;
            product.Price = model.Price;
            product.QuantityAvailable = model.QuantityAvailable;

            // Only change image if they provided a new URL or uploaded a file
            var newImage = await ResolveImagePath(model);
            if (!string.IsNullOrWhiteSpace(newImage))
            {
                product.ImagePath = newImage;
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<string?> ResolveImagePath(AdminProductsEditViewModel model)
        {
            if(!string.IsNullOrWhiteSpace(model.ImageUrl))
                return model.ImageUrl.Trim();
            if(model.ImageFile != null && model.ImageFile.Length > 0)
            {
                var uploadsFolder = Path.Combine(_env.WebRootPath, "images", "products");
                Directory.CreateDirectory(uploadsFolder);


                var safeFileName = $"{Guid.NewGuid()}{Path.GetExtension(model.ImageFile.FileName)}";
                var fullPath = Path.Combine(uploadsFolder, safeFileName);

                using var stream = new FileStream(fullPath, FileMode.Create);
                await model.ImageFile.CopyToAsync(stream);

                return $"/images/products/{safeFileName}";
            }

            return null;

        }

    }
}
