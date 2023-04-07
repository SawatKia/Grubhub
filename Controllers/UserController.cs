using Grubhub.Data;
using Grubhub.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
        public IActionResult Index(string username)
        {
            ViewBag.Username = username;
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult index(User obj)
        {
            ViewBag.Username = obj.Username;
            return View();
        }
        public IActionResult LoginRegis()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult LoginRegis(User obj)
        {
            var existingUser = _db.UsersData.Any(u =>
                (u.Username != null && u.Username.Equals(obj.Username)) ||
                (u.Email != null && u.Email.Equals(obj.Email))
            );

            if (existingUser)
            {
                //ModelState.AddModelError("RegisterUsername", "A user with the same username or email already exists.");
                TempData["RegisterationMessage"] = "A user with the same username or email already exists.";
                return View();
            }

            if (obj.Password != obj.ConfirmPassword)
            {
                //ModelState.AddModelError("RegisterPW", "The password and confirmation password do not match.");
                TempData["RegisterationMessage"] = "The password and confirmation password do not match.";
                return View();
            }
            // hash the password using BCrypt
            //var salt = Bcrypt.GenerateSalt();
            //var passwordHash = Bcrypt.HashPassword(obj.Password, salt);

            //obj.PasswordHash = passwordHash;
            //obj.salt = salt;

            _db.UsersData.Add(obj);
            _db.SaveChanges();
            // Pass the username to the Index action using a route parameter
            return RedirectToAction("Index", new { username = obj.Username });
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

            // authentication successful, store user id in session or cookie
            HttpContext.Session.SetString("userid", user.Id.ToString());

            // redirect to homepage or dashboard
            return RedirectToAction("index", "home");
            //try
            //{
            //    User user = _db.UsersData.Single(u => u.Username == username && u.Password == password);

            //    if (user.Password != password)
            //    {
            //        TempData["LoginMessage"] = "Invalid password.try again";
            //        return View("LoginRegis");
            //    }

            //    // Authentication successful, store user ID in session or cookie
            //    HttpContext.Session.SetString("UserId", user.Id.ToString());

            //    // Redirect to homepage or dashboard
            //    return RedirectToAction("Index", "Home");
            //}
            //catch (SqlNullValueException)
            //{
            //    TempData["LoginMessage"] = "Invalid username or password.";
            //    return View("LoginRegis");
            //}
        }


    }
}
