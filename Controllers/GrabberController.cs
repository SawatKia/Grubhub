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

            var viewModel = new UserPostsViewModel
			{
                CustomerOrders = user_order,
				User = user,
				Posts = posts
			};

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
            var postToDelete = _db.GrabberPostingField.FirstOrDefault(p => p.PostId == postID);
            if (postToDelete != null)
            {
                var orderToDelete = _db.CustomerOrders.FirstOrDefault(o => o.PostId == postID);
                if (orderToDelete != null)
                {
                    _db.CustomerOrders.Remove(orderToDelete);
                }
                _db.GrabberPostingField.Remove(postToDelete);
                _db.SaveChanges();
            }
            var posts = _db.GrabberPostingField.ToList();
            return RedirectToAction("Index", "Grabber", posts);
        }
    }


}

