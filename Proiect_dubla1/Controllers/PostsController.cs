using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Proiect_dubla1.Data;   // schimbă dacă DbContext e în alt namespace
using Proiect_dubla1.Models;

namespace Proiect_dubla1.Controllers
{
    [Authorize]
    public class PostsController : Controller
    {
        private readonly AppDbContext _db;
        private readonly IWebHostEnvironment _env;

        public PostsController(AppDbContext db, IWebHostEnvironment env)
        {
            _db = db;
            _env = env;
        }

        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            var posts = await _db.Posts
                .Include(p => p.User)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();

            return View(posts);
        }
        [Authorize]
        public async Task<IActionResult> ForYou()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            // ia ID-urile userilor pe care îi urmărești
            var followingIds = await _db.Follows
                .Where(f => f.FollowerId == userId)
                .Select(f => f.FollowedId)
                .ToListAsync();

            var posts = await _db.Posts
                .Include(p => p.User)
                .Where(p => followingIds.Contains(p.UserId))
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();

            return View(posts);
        }
        // -------- THOUGHT (TEXT) --------
        public IActionResult CreateThought()
        {
            return RedirectToAction(nameof(Index));

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateThought(string content)
        {
            if (string.IsNullOrWhiteSpace(content))
            {
                ModelState.AddModelError("", "Textul nu poate fi gol.");
                return View();
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            var post = new Post
            {
                Type = PostType.Thought,
                Content = content.Trim(),
                CreatedAt = DateTime.UtcNow,
                UserId = userId,
                Caption = null,
                ImagePath = null
            };

            _db.Posts.Add(post);
            await _db.SaveChangesAsync();
            return RedirectToAction("Details", "Users", new { id = userId });
        }

        // -------- IMAGE POST --------
        public IActionResult CreatePost()
        {
            return View();
        }
        

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreatePost(IFormFile image, string? caption)
        {
            if (image == null || image.Length == 0)
            {
                ModelState.AddModelError("", "Trebuie să alegi o imagine.");
                return View();
            }
            var allowed = new[] { ".jpg", ".jpeg", ".png", ".webp" };
            var ext = Path.GetExtension(image.FileName).ToLowerInvariant();
            if (!allowed.Contains(ext))
            {
                ModelState.AddModelError("", "Format invalid. Folosește JPG, PNG sau WEBP.");
                return View();
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            // 1) salvează poza în wwwroot/uploads
            var uploadsDir = Path.Combine(_env.WebRootPath, "uploads");
            Directory.CreateDirectory(uploadsDir);

            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(image.FileName)}";
            var filePath = Path.Combine(uploadsDir, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
                await image.CopyToAsync(stream);

            // 2) salvează în DB path-ul
            var post = new Post
            {
                Type = PostType.ImagePost,
                ImagePath = $"/uploads/{fileName}",
                Caption = string.IsNullOrWhiteSpace(caption) ? null : caption.Trim(),
                CreatedAt = DateTime.UtcNow,
                UserId = userId,
                Content = null
            };

            _db.Posts.Add(post);
            await _db.SaveChangesAsync();
            return RedirectToAction("Details", "Users", new { id = userId });
        }
    }
}
