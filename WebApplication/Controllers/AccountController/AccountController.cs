using Microsoft.AspNetCore.Mvc;

namespace WebApplication.Controllers.AccountController
{
    public class AccountController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
