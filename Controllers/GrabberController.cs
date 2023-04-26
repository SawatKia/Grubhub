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
            var user_order = _db.CustomerOrders.ToList();
            var currentTime = DateTime.Now;
            foreach (var post in posts)
            {
                if (post.CloseTime != null && currentTime > post.CloseTime)
                {
                    // Post has expired, remove it from the database
                    _db.GrabberPostingField.Remove(post);
                    _db.SaveChanges();
                }
            }
            posts = _db.GrabberPostingField
                .Where(p => p.UserId == userId)
                .ToList();
            var viewModel = new UserPostsViewModel
			{
                CustomerOrders = user_order,
				User = user,
				Posts = posts
			};
            ViewData["Controller"] = user.Roles;

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Index(GrabberPost obj)
        {
            var user_order = _db.CustomerOrders.ToList();
            var posts = _db.GrabberPostingField
                .Where(p => p.UserId == obj.UserId)
                .ToList();
            User userData = _db.UsersData.FirstOrDefault(u => u.Id == obj.UserId);
            ViewData["Controller"] = userData.Roles;
            var viewModel = new UserPostsViewModel
            {
                CustomerOrders =  user_order,
                User = userData,
                Posts = posts
            };
            if (posts.Count > 0)
            {
                TempData["Posting_Error"] = "you cannot place more than 1 post at a time.please close current post and try again.";
                return View(viewModel);
            }
            
            if (obj.CloseTime == null)
            {
                obj.CloseTime = DateTime.MaxValue;
            }
            _db.GrabberPostingField.Add(obj);
            _db.SaveChanges();

            posts = _db.GrabberPostingField
                .Where(p => p.UserId == obj.UserId)
                .ToList();
            user_order = _db.CustomerOrders.ToList();

            viewModel = new UserPostsViewModel
            {
                CustomerOrders = user_order,
                User = userData,
                Posts = posts
            };
            ViewData["Controller"] = userData.Roles;
            return View(viewModel);
        }
        public IActionResult Logout()
        {
            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("LoginRegis", "User");
        }
        [HttpPost]
        public IActionResult deletePost(int postID)
        {
            // Find the UserId associated with the given PostId
            var userId = _db.GrabberPostingField
                .Where(p => p.PostId == postID)
                .Select(p => p.UserId)
                .FirstOrDefault();
            var user = _db.UsersData.FirstOrDefault(o => o.Id == userId);
            var postsToDelete = _db.GrabberPostingField.Where(p => p.PostId == postID);
            var orderToDelete = _db.CustomerOrders.Where(o => o.PostId == postID);
            foreach (var order in orderToDelete)
            {
                _db.CustomerOrders.Remove(order);
            }
            foreach (var post in postsToDelete)
            {
                _db.GrabberPostingField.Remove(post);
            }
            _db.SaveChanges();
            ViewData["Controller"] = "Grabber";
            var posts = _db.GrabberPostingField.ToList();
            return RedirectToAction("Index", "Grabber", user);
        }
        [HttpGet]
        public IActionResult Profile(int Id)
        {
            
            User userData = _db.UsersData.FirstOrDefault(u => u.Id == Id); // Replace userId with the actual ID of the logged in user
            if (userData == null)
            {
                return NotFound();
            }
            ViewData["Controller"] = userData.Roles;
            return View(userData);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Profile(User Updateduser)
        {
            User existingUser = _db.UsersData.FirstOrDefault(u => u.Id == Updateduser.Id); // Get the existing user from the database
            if (existingUser == null)
            {
                return NotFound();
            }
            if (existingUser.Roles != Updateduser.Roles) {
                existingUser.Roles = Updateduser.Roles; // Update the roles
                var posts = _db.GrabberPostingField.Where(p => p.UserId == Updateduser.Id).ToList();
                if (posts.Count > 0)
                {
                    _db.GrabberPostingField.RemoveRange(posts);
                }

            }
            _db.SaveChanges(); // Save changes to the database
            ViewData["Controller"] = existingUser.Roles;
            return RedirectToAction("Index", existingUser.Roles, existingUser); // Redirect to the profile page
        }
    }


}

