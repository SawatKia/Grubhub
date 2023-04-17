using Grubhub.Data;
using Grubhub.Migrations;
using Grubhub.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;

namespace Grubhub.Controllers
{
    public class GrabberController : Controller
    {
        //DB representor
        private readonly GrubhubDBContext _db;

        public GrabberController(GrubhubDBContext db)
        {
            _db = db;
        }
        public IActionResult Index(User user)
        {
			var userId = user.Id; // replace with the actual user ID you want to search for
			var posts = _db.GrabberPostingField
				.Where(p => p.UserId == userId)
				.ToList();

			var viewModel = new UserPostsViewModel
			{
				User = user,
				Posts = posts
			};

			return View(viewModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Index(GrabberPost obj)
        {
            
            User userData = _db.UsersData.FirstOrDefault(u => u.Id == obj.UserId);
            obj.CloseTime = DateTime.MaxValue;
            _db.GrabberPostingField.Add(obj);
            _db.SaveChanges();

            var posts = _db.GrabberPostingField
                .Where(p => p.UserId == obj.UserId)
                .ToList();

            var viewModel = new UserPostsViewModel
            {
                User = userData,
                Posts = posts
            };
            return View(viewModel);
        }
        public IActionResult Logout()
        {
            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("LoginRegis", "User");
        }

    }
}
