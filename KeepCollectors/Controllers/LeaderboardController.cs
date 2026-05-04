using DataAccessLayer.Data;
using KeepCollectors.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KeepCollectors.Controllers
{
    public class LeaderboardController : Controller
    {
        private readonly CollectorsKeepDbContext _context;

        public LeaderboardController(CollectorsKeepDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var purchasedData = await _context.OrderItems
                .Include(oi => oi.Order)
                .Include(oi => oi.Product)
                .Where(oi => oi.Order != null && oi.Order.Customer != null)
                .GroupBy(oi => new
                {
                    oi.Order.CustomerID,
                    oi.Order.Customer.UserName
                })
                .Select(g => new
                {
                    UserId = g.Key.CustomerID,
                    UserName = g.Key.UserName,
                    TotalItems = g.Sum(x => x.Quantity),
                    TotalValue = g.Sum(x => x.Product != null
                        ? x.Product.Price * x.Quantity
                        : x.UnitPrice * x.Quantity)
                })
                .ToListAsync();

            var savedData = await _context.WishlistItems
                .Include(wi => wi.Wishlist)
                .Include(wi => wi.Product)
                .Where(wi => wi.Wishlist != null && wi.Wishlist.Customer != null)
                .GroupBy(wi => new
                {
                    wi.Wishlist.CustomerID,
                    wi.Wishlist.Customer.UserName
                })
                .Select(g => new
                {
                    UserId = g.Key.CustomerID,
                    UserName = g.Key.UserName,
                    TotalItems = g.Count(),
                    TotalValue = g.Sum(x => x.Product != null ? x.Product.Price : 0)
                })
                .ToListAsync();

            var leaderboardData = purchasedData
                .Concat(savedData)
                .GroupBy(x => new { x.UserId, x.UserName })
                .Select(g => new LeaderboardViewModel
                {
                    UserName = g.Key.UserName ?? "Unknown User",
                    totalItems = g.Sum(x => x.TotalItems),
                    TotalValue = g.Sum(x => x.TotalValue)
                })
                .OrderByDescending(x => x.TotalValue)
                .ThenByDescending(x => x.totalItems)
                .ToList();

            for (int i = 0; i < leaderboardData.Count; i++)
            {
                leaderboardData[i].Rank = i + 1;
            }

            return View(leaderboardData);
        }
    }
}
