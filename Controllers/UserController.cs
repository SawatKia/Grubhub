using Microsoft.AspNetCore.Mvc;

namespace Grubhub.Controllers
{
	public class UserController : Controller
	{
		public IActionResult Index()
		{
			return View();
		}
	}
}
