using Microsoft.AspNetCore.Mvc;
using DataAccessLayer.Data;
using DataAccessLayer.DataModels;
using KeepCollectors.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace KeepCollectors.Controllers
{
    public class KeepController : Controller
    {
       
            private readonly CollectorsKeepDbContext _context;
            private readonly UserManager<ApplicationUser> _userManager;

            public KeepController(CollectorsKeepDbContext context, UserManager<ApplicationUser> userManager)
            {
                _context = context;
                _userManager = userManager;
            }

            public async Task<IActionResult> Index()
            {
                var user = await _userManager.GetUserAsync(User);

                var purchasedItems = await _context.OrderItems
                    .Include(oi => oi.Product)
                    .Include(oi => oi.Order)
                    .Where(oi => oi.Order.CustomerID == user.Id)
                    .OrderByDescending(oi => oi.Order.DateOrdered)
                    .ToListAsync();

                var savedItems = await _context.WishlistItems
                    .Include(wi => wi.Product)
                    .Include(wi => wi.Wishlist)
                    .Where(wi => wi.Wishlist.CustomerID == user.Id)
                    .OrderByDescending(wi => wi.DateAdded)
                    .ToListAsync();

            var collectionWorth = purchasedItems
                .Where(item => item.Product != null)
                .Sum(item => item.Product!.Price * item.Quantity);


            var savedItemsWorth = savedItems
                .Where(item => item.Product != null)
                .Sum(item => item.Product!.Price);

            return View(new KeepViewModel
            {
                PurchasedItems = purchasedItems,
                SavedItems = savedItems,
                CollectionWorth = collectionWorth,
                SavedItemsWorth = savedItemsWorth
            });

        }

        public async Task<IActionResult> Add(int productId)
            {
                var user = await _userManager.GetUserAsync(User);

                var wishlist = await _context.Wishlists
                    .Include(w => w.WishlistItems)
                    .FirstOrDefaultAsync(w => w.CustomerID == user.Id);

                if (wishlist == null)
                {
                    wishlist = new Wishlist
                    {
                        CustomerID = user.Id
                    };

                    _context.Wishlists.Add(wishlist);
                    await _context.SaveChangesAsync();
                }

                var alreadySaved = wishlist.WishlistItems.Any(wi => wi.ProductID == productId);

                if (!alreadySaved)
                {
                    wishlist.WishlistItems.Add(new WishlistItem
                    {
                        ProductID = productId
                    });

                    await _context.SaveChangesAsync();
                }

                return RedirectToAction(nameof(Index));
            }

            public async Task<IActionResult> RemoveSaved(int id)
            {
                var user = await _userManager.GetUserAsync(User);

                var item = await _context.WishlistItems
                    .Include(wi => wi.Wishlist)
                    .FirstOrDefaultAsync(wi => wi.WishlistItemID == id && wi.Wishlist.CustomerID == user.Id);

                if (item != null)
                {
                    _context.WishlistItems.Remove(item);
                    await _context.SaveChangesAsync();
                }

                return RedirectToAction(nameof(Index));
            }
        }
    }

