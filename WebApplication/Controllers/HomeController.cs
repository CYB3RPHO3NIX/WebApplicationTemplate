using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace WebApplication.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View(@"Views\Home\Index.cshtml");
        }
        public IActionResult Home()
        {
            return View(@"Views\Home\Home.cshtml");
        }
        [Authorize(Roles = "USER,ADMINISTRATOR")]
        public IActionResult UserPage()
        {
            return View(@"Views\Home\UserArea.cshtml");
        }
        [Authorize(Roles = "ADMINISTRATOR")]
        public IActionResult AdminPage()
        {
            return View(@"Views\Home\AdminArea.cshtml");
        }
    }
}
