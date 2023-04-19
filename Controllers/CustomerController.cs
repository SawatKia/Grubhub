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
            _db.GrabberPostingField.RemoveRange(postsToRemove);
            _db.SaveChanges();
            var posts = _db.GrabberPostingField.ToList();
            var user_order = _db.CustomerOrders.ToList();
            var viewModel = new UserPostsViewModel
            {
                CustomerOrders = user_order,
                User = user_obj,
                Posts = posts
            };
            return View(viewModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Index(CustomerOrder order)
        {
            var post = _db.GrabberPostingField.FirstOrDefault(p => p.PostId == order.PostId);

            if (order.NumBoxes > post.MaxQuantity)
            {
                ModelState.AddModelError("NumBoxes", "Number of boxes cannot exceed " + post.MaxQuantity + ".");
            }

            if (order.EstimatedTotalPrice > Convert.ToDecimal(post.MaxTotalPrice))
            {
                ModelState.AddModelError("EstimatedTotalPrice", "Estimated total price cannot exceed " + post.MaxTotalPrice + ".");
            }

            if (!ModelState.IsValid)
            {
                var posts = _db.GrabberPostingField.ToList();
                var user_order = _db.CustomerOrders.ToList();
                var user_obj = _db.UsersData.FirstOrDefault(o => o.Id == order.CustomerId);

                var viewModel = new UserPostsViewModel
                {
                    CustomerOrders = user_order,
                    User = user_obj,
                    Posts = posts
                };
                // Validation failed, return the same view with validation errors
                return View(viewModel);
            }
            _db.CustomerOrders.Add(order);
            _db.SaveChanges();
            //show comment
            
            return RedirectToAction("Index");
        }
    }
}