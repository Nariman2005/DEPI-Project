using Microsoft.AspNetCore.Mvc;

namespace WebApplication1.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult Login()
        {
            return View();
        }

        public IActionResult Dashboard()
        {
            return View();
        }


    }
}
