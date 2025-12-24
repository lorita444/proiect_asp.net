using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Proiect_dubla1.Data;
using Proiect_dubla1.Models;
using Microsoft.AspNetCore.Identity;

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
    public IActionResult Details(string id)
    {
        var user = _context.Users.FirstOrDefault(u => u.Id == id);

        if (user == null)
            return NotFound();

        // Obține postările utilizatorului
        var posts = _context.Posts
            .Where(p => p.UserId == id)
            .ToList();

        ViewBag.Posts = posts;

        return View(user);
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
    





}
