using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Proiect_dubla1.Data;
using Proiect_dubla1.Models;

namespace Proiect_dubla1.Controllers
{
    [Authorize]
    public class FollowsController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<User> _userManager;

        public FollowsController(AppDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Follow(string id) // id = userul urmarit (FollowedId)
        {
            var currentUserId = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(currentUserId)) return Unauthorized();
            if (currentUserId == id) return BadRequest();

            var already = _context.Follows.Any(f =>
                f.FollowerId == currentUserId && f.FollowedId == id);

            if (!already)
            {
                _context.Follows.Add(new Follow
                {
                    FollowerId = currentUserId,
                    FollowedId = id
                });
                _context.SaveChanges();
            }

            return RedirectToAction("Details", "Users", new { id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Unfollow(string id)
        {
            var currentUserId = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(currentUserId)) return Unauthorized();
            if (currentUserId == id) return BadRequest();

            var rel = _context.Follows.FirstOrDefault(f =>
                f.FollowerId == currentUserId && f.FollowedId == id);

            if (rel != null)
            {
                _context.Follows.Remove(rel);
                _context.SaveChanges();
            }

            return RedirectToAction("Details", "Users", new { id });
        }
    }
}
