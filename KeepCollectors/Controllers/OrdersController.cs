using DataAccessLayer.Data;
using DataAccessLayer.DataModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

public class OrdersController : Controller
{
    private readonly CollectorsKeepDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public OrdersController(CollectorsKeepDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    // ✅ LOAD CHECKOUT PAGE
    [Authorize]
    public async Task<IActionResult> Checkout()
    {
        var user = await _userManager.GetUserAsync(User);

        var cart = await _context.Carts
            .Include(c => c.CartItems)
            .ThenInclude(ci => ci.Product)
            .FirstOrDefaultAsync(c => c.CustomerID == user.Id);

        return View(cart);
    }

    // ✅ PLACE ORDER (MAIN LOGIC)
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> PlaceOrder()
    {
        var user = await _userManager.GetUserAsync(User);

        var cart = await _context.Carts
            .Include(c => c.CartItems)
            .ThenInclude(ci => ci.Product)
            .FirstOrDefaultAsync(c => c.CustomerID == user.Id);

        if (cart == null || !cart.CartItems.Any())
        {
            return RedirectToAction("Index", "Cart");
        }

        var order = new Order
        {
            CustomerID = user.Id,
            DateOrdered = DateTime.Now,
            OrderItems = new List<OrderItem>()
        };

        foreach (var item in cart.CartItems)
        {
            var product = item.Product;

            if (product.QuantityAvailable < item.Quantity)
            {
                TempData["Error"] = $"Not enough stock for {product.Name}";
                return RedirectToAction("Checkout");
            }

            // 🔥 Update inventory
            product.QuantityAvailable -= item.Quantity;

            order.OrderItems.Add(new OrderItem
            {
                ProductID = product.ProductID,
                Quantity = item.Quantity,
                UnitPrice = product.Price
            });

        }

        _context.Orders.Add(order);

        // 🔥 CLEAR CART
        _context.CartItems.RemoveRange(cart.CartItems);

        await _context.SaveChangesAsync();

        return RedirectToAction("Confirmation");
    }

    // ✅ CONFIRMATION PAGE
    [Authorize]
    public IActionResult Confirmation()
    {
        return View();
    }

    [Authorize]
    public async Task<IActionResult> History()
    {
        var user = await _userManager.GetUserAsync(User);

        var orders = await _context.Orders
            .Where(o => o.CustomerID == user.Id)
            .Include(o => o.OrderItems)
            .ThenInclude(oi => oi.Product)
            .OrderByDescending(o => o.DateOrdered)
            .ToListAsync();

        return View(orders);
    }
}