using Assignment.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.DotNet.Scaffolding.Shared.Messaging;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace Assignment.Controllers
{
	public class HomeController : Controller
	{
		NorthwindContext context = new NorthwindContext();
		private readonly ILogger<HomeController> _logger;

		public HomeController(ILogger<HomeController> logger)
		{
			_logger = logger;
		}

		public IActionResult Index()
		{
			return View();
		}
        public IActionResult Logout()
        {
			HttpContext.Session.Clear();
            IHttpContextAccessor _httpContextAccessor = new HttpContextAccessor();
			_httpContextAccessor.HttpContext.Session.Clear();
            return View("Index");
        }

        public IActionResult DoLogin(string username, string pass)
		{
			if(context.Accounts.Find(username) == null)
			{
				ViewBag.Error = "Username is not existence";
			}
			else if(context.Accounts.FirstOrDefault(x => x.Id == username && x.Password == pass) == null) 
			{
                ViewBag.Error = "Password is wrong";
			}
			else if(context.Accounts.FirstOrDefault(x => x.Id == username && x.Password == pass) != null)
			{
				HttpContext.Session.SetString("Username", context.Accounts.FirstOrDefault(x => x.Id == username && x.Password == pass).Id);
				if(context.Accounts.FirstOrDefault(x => x.Id == username && x.Password == pass).CustomerId != null)
				{
                    return RedirectToAction("List", "Product");
                }
                return RedirectToAction("Index", "Admin");
            }
			return View("Index");
		}


		public IActionResult Privacy()
		{
			return View();
		}

		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error()
		{
			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}
	}
}