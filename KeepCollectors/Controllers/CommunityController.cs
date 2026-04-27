using DataAccessLayer.Data;
using DataAccessLayer.DataModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace KeepCollectors.Controllers
{
    public class CommunityController : Controller
    {
        private readonly CollectorsKeepDbContext _context;

        public CommunityController(CollectorsKeepDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var posts = _context.Posts
                .Include(p => p.User)
                .Include(p => p.Comments)
                .Include(p => p.Likes)
                .OrderByDescending(p => p.CreatedAt)
                .ToList();

            return View(posts);
        }

        [HttpPost]
        public async Task<IActionResult> CreatePost(string content, IFormFile image)
        {
            content = content?.Trim();

            if (string.IsNullOrEmpty(content) && image == null)
            {
                ModelState.AddModelError("", "Post must contain text or an image.");
                return RedirectToAction("Index");
            }

            string imagePath = null;

            if (image != null)
            {
                var fileName = Guid.NewGuid() + Path.GetExtension(image.FileName);
                var path = Path.Combine("wwwroot/images/posts", fileName);

                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await image.CopyToAsync(stream);
                }

                imagePath = "/images/posts/" + fileName;
            }

            var post = new Post
            {
                Content = content,
                ImagePath = imagePath,
                UserId = User.FindFirstValue(System.Security.Claims.ClaimTypes.NameIdentifier),
                CreatedAt = DateTime.Now
            };

            _context.Posts.Add(post);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Like(int postId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var existing = _context.Likes
                .FirstOrDefault(l => l.PostId == postId && l.UserId == userId);

            if (existing == null)
            {
                _context.Likes.Add(new Like { PostId = postId, UserId = userId });
                _context.SaveChanges();
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult AddComment(int postId, string content)
        {
            var comment = new Comment
            {
                PostId = postId,
                Content = content,
                UserId = User.FindFirstValue(ClaimTypes.NameIdentifier),
                CreatedAt = DateTime.Now
            };

            _context.Comments.Add(comment);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> DeletePost(int postId)
        {
            var post = await _context.Posts.FindAsync(postId);

            if (post == null)
                return NotFound();

            var userId = User.FindFirstValue(System.Security.Claims.ClaimTypes.NameIdentifier);
            
            if (post.UserId != userId)
                return Forbid();

            _context.Posts.Remove(post);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }
    }
}
