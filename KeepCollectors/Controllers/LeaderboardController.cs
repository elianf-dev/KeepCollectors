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
            var leaderboardData = await _context.UserKeepItems
                .Include(uki => uki.User)
                .GroupBy(uki => new {uki.UserKeepItemID, uki.User.UserName })
                .Select(g => new LeaderboardViewModel
                {
                    UserName = g.Key.UserName,
                    totalItems = g.Sum(x => x.Quantity),
                    TotalValue = g.Sum(x => x.ItemValue * x.Quantity),

                })
                .OrderByDescending(x => x.TotalValue)
                .ThenByDescending(x => x.totalItems)
                .ToListAsync();

            for(int i = 0; i < leaderboardData.Count; i++)
            {
                leaderboardData[i].Rank = i + 1;
            }
            return View(leaderboardData);
        }
    }
}
