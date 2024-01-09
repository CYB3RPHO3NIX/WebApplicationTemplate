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
    }
}
