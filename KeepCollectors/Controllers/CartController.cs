using DataAccessLayer.Data;
using DataAccessLayer.DataModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[Authorize]
public class CartController : Controller
{
    private readonly CollectorsKeepDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public CartController(CollectorsKeepDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return Challenge();
        }

        var cart = await _context.Carts
            .Include(c => c.CartItems)
            .ThenInclude(ci => ci.Product)
            .FirstOrDefaultAsync(c => c.CustomerID == user.Id);

        if (cart == null)
        {
            cart = new Cart
            {
                CustomerID = user.Id,
                CartItems = new List<CartItem>()
            };

            _context.Carts.Add(cart);
            await _context.SaveChangesAsync();
        }

        return View(cart);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddToCart(int productId, int quantity)
    {
        if (quantity < 1)
        {
            quantity = 1;
        }

        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return Challenge();
        }

        var product = await _context.Products.FirstOrDefaultAsync(p => p.ProductID == productId);
        if (product == null)
        {
            return NotFound();
        }

        var cart = await _context.Carts
            .Include(c => c.CartItems)
            .FirstOrDefaultAsync(c => c.CustomerID == user.Id);

        if (cart == null)
        {
            cart = new Cart
            {
                CustomerID = user.Id,
                CartItems = new List<CartItem>()
            };

            _context.Carts.Add(cart);
            await _context.SaveChangesAsync();
        }

        var existingCartItem = cart.CartItems.FirstOrDefault(ci => ci.ProductID == productId);

        int currentQuantityInCart = existingCartItem?.Quantity ?? 0;
        int newRequestedTotal = currentQuantityInCart + quantity;

        if (newRequestedTotal > product.QuantityAvailable)
        {
            TempData["Error"] = $"Only {product.QuantityAvailable} available for {product.Name}.";
            return RedirectToAction("Details", "Products", new { id = productId });
        }

        if (existingCartItem != null)
        {
            existingCartItem.Quantity += quantity;
        }
        else
        {
            var cartItem = new CartItem
            {
                CartID = cart.CartID,
                ProductID = productId,
                Quantity = quantity
            };

            _context.CartItems.Add(cartItem);
        }

        await _context.SaveChangesAsync();

        TempData["Success"] = $"{product.Name} added to cart.";
        return RedirectToAction("Index", "Cart");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Remove(int cartItemId)
    {
        var cartItem = await _context.CartItems.FirstOrDefaultAsync(ci => ci.CartItemID == cartItemId);

        if (cartItem != null)
        {
            _context.CartItems.Remove(cartItem);
            await _context.SaveChangesAsync();
        }

        return RedirectToAction("Index");
    }
}