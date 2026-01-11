using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Proiect_dubla1.Data;
using Proiect_dubla1.Models;
using System.Security.Claims;
using UserEntity = Proiect_dubla1.Models.User;

public class UsersController : Controller
{
    private readonly AppDbContext _context;
    private readonly UserManager<UserEntity> _userManager;

    public UsersController(AppDbContext context, UserManager<UserEntity> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    // get users
    public IActionResult Index(string search)
    {
        IQueryable<UserEntity> query = _context.Users;

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.ToLower();
            query = query.Where(u =>
                (u.FirstName + " " + u.LastName).ToLower().Contains(term));
        }

        var users = query
            .OrderBy(u => u.FirstName)
            .ThenBy(u => u.LastName)
            .ToList();

        ViewBag.Search = search;

        return View(users);
    }

    //GET: /Users/Details/stringId
    public IActionResult Details(string? id)
    {
        // Dacă cineva intră pe /Users/Details fără id, îl ducem pe profilul lui (dacă e logat)
        if (string.IsNullOrWhiteSpace(id))
        {
            var currentUserId = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(currentUserId))
                return RedirectToAction("Login", "Account"); // sau Home/Index, cum vrei

            return RedirectToAction(nameof(Details), new { id = currentUserId });
        }

        var profile = _context.Users.FirstOrDefault(u => u.Id == id);
        if (profile == null) return NotFound();

        var currentUserId2 = _userManager.GetUserId(User);

        ViewBag.IsOwnProfile = !string.IsNullOrEmpty(currentUserId2) && currentUserId2 == id;

        ViewBag.IsFollowing = !string.IsNullOrEmpty(currentUserId2) &&
            _context.Follows.Any(f => f.FollowerId == currentUserId2 && f.FollowedId == id);

        ViewBag.FollowersCount = _context.Follows.Count(f => f.FollowedId == id);
        ViewBag.FollowingCount = _context.Follows.Count(f => f.FollowerId == id);

        ViewBag.Posts = _context.Posts
            .Where(p => p.UserId == id)
            .OrderByDescending(p => p.CreatedAt)
            .ToList();

        return View(profile);
    }


    public IActionResult Edit(string id)
    {
        var user = _context.Users.FirstOrDefault(u => u.Id == id);

        if (user == null)
            return NotFound();

        return View(user);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(User model, IFormFile profileImage)
    {
        var user = _context.Users.FirstOrDefault(u => u.Id == model.Id);
        if (user == null)
            return NotFound();

        user.FirstName = model.FirstName;
        user.LastName = model.LastName;
        user.UserName = model.UserName;
        user.Description = model.Description;
        user.RelationshipStatus = model.RelationshipStatus;
        user.City = model.City;
        user.Country = model.Country;
        user.Age = model.Age;

        if (profileImage != null && profileImage.Length > 0)
        {
            var ext = Path.GetExtension(profileImage.FileName).ToLower();

            // Acceptăm DOAR imagini
            if (ext != ".jpg" && ext != ".jpeg" && ext != ".png")
            {
                ModelState.AddModelError("", "Poza trebuie să fie JPG sau PNG.");
                return View(model);
            }

            var fileName = Guid.NewGuid() + ext;
            var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img", fileName);

            using (var stream = new FileStream(path, FileMode.Create))
            {
                await profileImage.CopyToAsync(stream);
            }

            user.ProfileImagePath = "/img/" + fileName;
        }

        _context.SaveChanges();
        return RedirectToAction("Details", new { id = user.Id });
    }


    public IActionResult Delete(string id)
    {
        var user = _context.Users.FirstOrDefault(u => u.Id == id);

        if (user == null)
            return NotFound();

        return View(user);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult DeleteConfirmed(string id)
    {
        var user = _context.Users.FirstOrDefault(u => u.Id == id);

        if (user == null)
            return NotFound();

        _context.Users.Remove(user);
        _context.SaveChanges();

        return RedirectToAction("Index");
    }

    public async Task<IActionResult> Followers(string id)
    {
        if (string.IsNullOrWhiteSpace(id)) return NotFound();

        var me = User.FindFirstValue(ClaimTypes.NameIdentifier); // poate fi null dacă nu e logat

        // user-ul al cărui profil îl vezi
        var profile = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
        if (profile == null) return NotFound();

        // lista followerilor (cei care au FollowedId = id)
        var followers = await _context.Follows
            .Where(f => f.FollowedId == id)
            .Join(_context.Users,
                  f => f.FollowerId,
                  u => u.Id,
                  (f, u) => new FollowerVm
                  {
                      UserId = u.Id,
                      UserName = u.UserName,
                      ProfileImagePath = u.ProfileImagePath,
                      IsFollowedByMe = me != null && _context.Follows.Any(x => x.FollowerId == me && x.FollowedId == u.Id)
                  })
            .ToListAsync();

        ViewBag.ProfileUser = profile;  // ca să afișezi “Followers of X”
        return View(followers);
    }
    public async Task<IActionResult> Following(string id)
    {
        if (string.IsNullOrWhiteSpace(id)) return NotFound();

        var me = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var profile = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
        if (profile == null) return NotFound();

        var following = await _context.Follows
            .Where(f => f.FollowerId == id)
            .Join(_context.Users,
                  f => f.FollowedId,
                  u => u.Id,
                  (f, u) => new FollowerVm
                  {
                      UserId = u.Id,
                      UserName = u.UserName,
                      ProfileImagePath = u.ProfileImagePath,
                      IsFollowedByMe = me != null && _context.Follows.Any(x => x.FollowerId == me && x.FollowedId == u.Id)
                  })
            .ToListAsync();

        ViewBag.ProfileUser = profile;
        return View(following);
    }




}
