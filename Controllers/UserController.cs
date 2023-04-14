using Grubhub.Data;
using Grubhub.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;

namespace Grubhub.Controllers
{
    public class UserController : Controller
    {
        //DB representor
        private readonly GrubhubDBContext _db;

        public UserController(GrubhubDBContext db)
        {
            _db = db;
        }
        public IActionResult Index(User user)
        {
            ViewBag.Username = user.Username;
            ViewBag.Id = user.Id;
            ViewBag.Roles = user.Roles;
            
            return View();
        }
        public IActionResult LoginRegis()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        //registion handler
        public IActionResult LoginRegis(User obj)
        {
            if (string.IsNullOrEmpty(obj.Username) || string.IsNullOrEmpty(obj.Email) || string.IsNullOrEmpty(obj.Password) || string.IsNullOrEmpty(obj.ConfirmPassword))
            {
                TempData["RegisterationMessage"] = "Please fill in all the required fields.";
                return View();
            }
            var existingUser = _db.UsersData.Any(u =>
                (u.Username != null && u.Username.Equals(obj.Username)) ||
                (u.Email != null && u.Email.Equals(obj.Email))
            );

            if (existingUser)
            {
                //ModelState.AddModelError("RegisterUsername", "A user with the same username or email already exists.");
                TempData["RegisterationMessage"] = "A user with the same username or email already exists.";
                return View("LoginRegis", new { Panel = "right" });
            }

            if (obj.Password != obj.ConfirmPassword)
            {
                //ModelState.AddModelError("RegisterPW", "The password and confirmation password do not match.");
                TempData["RegisterationMessage"] = "The password and confirmation password do not match.";
                return View("LoginRegis", new { Panel = "right" });
            }
            obj.Roles = "default";
            // hash the password using BCrypt
            //var salt = Bcrypt.GenerateSalt();
            //var passwordHash = Bcrypt.HashPassword(obj.Password, salt);

            //obj.PasswordHash = passwordHash;
            //obj.salt = salt;

            _db.UsersData.Add(obj);
            _db.SaveChanges();

            // Retrieve the user from the database
            var user = _db.UsersData.SingleOrDefault(u => u.Username == obj.Username);

            // Pass the username to the Index action using a route parameter
            return RedirectToAction("SelectRoles", user) ;
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(string username, string password)
        {
            bool isvaliduser = _db.UsersData.Any(u => u.Username == username && u.Password == password);

            if (!isvaliduser)
            {
                TempData["loginmessage"] = "invalid username or password.";
                return View("loginregis");
            }

            var sql = $"SELECT * FROM UsersData WHERE Username = '{username}' AND Password = '{password}'";
            var user = _db.UsersData.FromSqlRaw(sql).SingleOrDefault();

            if (user == null)
            {
                TempData["loginmessage"] = "Invalid username or password.";
                return View("loginregis");
            }

            // region set session value
            // authentication successful, store user id in session or cookie
            HttpContext.Session.SetString("userid", user.Id.ToString());
            // end region
            if (user.Roles != "default")
            {
                // redirect to homepage or dashboard
                return RedirectToAction("index", user);
            }
            else//if user role == deafault then goto selectroles
            {
                ViewBag.Username = user.Username;
                return RedirectToAction("SelectRoles", "User", user);
            }
        }
        public IActionResult SelectRoles(User user)
        { 
            if (user == null)
            {
                // Handle the case where the user object is null
                return RedirectToAction("Login", "User");
            }
            ViewBag.Username = user.Username;
            return View(user);
        }
        [HttpPost]
        public IActionResult SelectRoles()
        {
            int userId = int.Parse(Request.Form["userId"]);
            string username = Request.Form["username"];
            string selectedRole = Request.Form["selectedRole"];

            var user = _db.UsersData.FirstOrDefault(u => u.Id == userId);

            if (user == null)
            {
                return NotFound();
            }
            user.Roles = selectedRole;
            _db.UsersData.Update(user);
            _db.SaveChanges();
            user = _db.UsersData.FirstOrDefault(u => u.Id == userId);

            return RedirectToAction("Index", "User", user);
        }




    }
}
