using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using DataAccessLayer.Data;
using DataAccessLayer.DataModels;

public class CartController : Controller
{
    private readonly CollectorsKeepDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public CartController(CollectorsKeepDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public async Task<IActionResult> AddToCart(int productId)
    {
        var user = await _userManager.GetUserAsync(User);

        var cart = await _context.Carts
            .Include(c => c.CartItems)
            .FirstOrDefaultAsync(c => c.CustomerID == user.Id);

        if (cart == null)
        {
            cart = new Cart { CustomerID = user.Id };
            _context.Carts.Add(cart);
            await _context.SaveChangesAsync();
        }

        var existingItem = cart.CartItems
            .FirstOrDefault(ci => ci.ProductID == productId);

        if (existingItem != null)
        {
            existingItem.Quantity++;
        }
        else
        {
            cart.CartItems.Add(new CartItem
            {
                ProductID = productId,
                Quantity = 1
            });
        }

        await _context.SaveChangesAsync();

        return RedirectToAction("Index");
    }

    public async Task<IActionResult> Index()
    {
        var user = await _userManager.GetUserAsync(User);

        var cart = await _context.Carts
            .Include(c => c.CartItems)
            .ThenInclude(ci => ci.Product)
            .FirstOrDefaultAsync(c => c.CustomerID == user.Id);

        return View(cart);
    }

    public async Task<IActionResult> Remove(int id)
    {
        var item = await _context.CartItems.FindAsync(id);

        if (item != null)
        {
            _context.CartItems.Remove(item);
            await _context.SaveChangesAsync();
        }

        return RedirectToAction("Index");
    }
}
