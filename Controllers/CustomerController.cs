﻿using Grubhub.Data;
using Grubhub.Migrations;
using Grubhub.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;

namespace Grubhub.Controllers
{
    public class CustomerController : Controller
    {
        //DB representor
        private readonly GrubhubDBContext _db;
        public CustomerController(GrubhubDBContext db)
        {
            _db = db;
        }
        public IActionResult Index(User user_obj)
        {
            // Retrieve the list of UserId values from GrabberPostingField table
            var userIds = _db.GrabberPostingField.Select(pf => pf.UserId).ToList();

            // Retrieve the list of Id values from UsersData table
            var ids = _db.UsersData.Select(u => u.Id).ToList();

            // Find the userId values that don't exist in ids and remove their corresponding rows from GrabberPostingField table
            var userIdsToRemove = userIds.Except(ids);
            var postsToRemove = _db.GrabberPostingField.Where(pf => userIdsToRemove.Contains(pf.UserId));
            if (postsToRemove != null) {
                _db.GrabberPostingField.RemoveRange(postsToRemove);
                _db.SaveChanges();
            }
            // Remove any expired posts from GrabberPostingField table
            var posts = _db.GrabberPostingField.ToList();
            var currentTime = DateTime.Now;
            //var expiredPosts = _db.GrabberPostingField.Where(pf => pf.CloseTime != null && currentTime >= pf.CloseTime);
            foreach (var post in posts)
            {
                if (post.CloseTime != null && currentTime >= post.CloseTime)
                {
                    // Post has expired, remove it from the database
                    _db.GrabberPostingField.Remove(post);
                    _db.SaveChanges();
                    //RedirectToAction("deletePost", "Grabber", post.PostId);
                }
            }
            //make it delete the orders that its PostId doesn't connect to any exisiting posts(PostId)
            // Retrieve the list of valid PostId values from GrabberPostingField table
            var validPostIds = _db.GrabberPostingField.Select(pf => pf.PostId).ToList();

            // Retrieve the list of CustomerOrders that have invalid PostId values and remove them
            var invalidOrders = _db.CustomerOrders.Where(co => !validPostIds.Contains(co.PostId));
            if (invalidOrders.Any())
            {
                _db.CustomerOrders.RemoveRange(invalidOrders);
                _db.SaveChanges();
            }
            posts = _db.GrabberPostingField.ToList();
            var user_order = _db.CustomerOrders.ToList();
            var viewModel = new UserPostsViewModel
            {
                CustomerOrders = user_order,
                User = user_obj,
                Posts = posts
            };
            ViewData["Controller"] = user_obj.Roles;
            return View(viewModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Index(CustomerOrder order)
        {
            var post = _db.GrabberPostingField.FirstOrDefault(p => p.PostId == order.PostId);
            var Errorcount = 0;
            var user_obj = _db.UsersData.FirstOrDefault(o => o.Id == order.CustomerId);

            if (order.NumBoxes > post.MaxQuantity)
            {
                Errorcount += 1;
            }

            if (order.EstimatedTotalPrice > Convert.ToDecimal(post.MaxTotalPrice))
            {
                Errorcount += 1;
            }

            if (Errorcount > 0)
            {
                var posts = _db.GrabberPostingField.ToList();
                var user_order = _db.CustomerOrders.ToList();
                

                var viewModel = new UserPostsViewModel
                {
                    CustomerOrders = user_order,
                    User = user_obj,
                    Posts = posts
                };
                ViewData["Controller"] = user_obj.Roles;
                // Validation failed, return the same view with validation errors
                return View(viewModel);
            }
            _db.CustomerOrders.Add(order);
            _db.SaveChanges();
            //show comment
            
            return RedirectToAction("Index",user_obj);
        }
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
            existingUser.Roles = Updateduser.Roles; // Update the roles
            _db.SaveChanges(); // Save changes to the database
            ViewData["Controller"] = existingUser.Roles;    
            return RedirectToAction("Index",existingUser.Roles,existingUser); // Redirect to the profile page
        }
    }
}
