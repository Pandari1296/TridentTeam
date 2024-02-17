using AspNetCoreHero.ToastNotification.Abstractions;
using BaseApplication.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace BaseApplication.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly INotyfService _notyf;

        public HomeController(ILogger<HomeController> logger, INotyfService notyf)
        {
            _logger = logger;
            _notyf = notyf;
        }

        public IActionResult Index()
        {
            return View();
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

        public IActionResult ContactUs()
        {
            return View();
        }

        public IActionResult Logout()
        {
            _notyf.Success("Logged out successfully!");
            HttpContext.Session.Clear();
            return RedirectToAction("Index");
        }

        public IActionResult DuoCallBack()
        {
            return View();
        }

    }
}
