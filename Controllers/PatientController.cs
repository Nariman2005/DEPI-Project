using Microsoft.AspNetCore.Mvc;

namespace WebApplication1.Controllers
{
    public class PatientController : Controller
    {
        public IActionResult Login()
        {
            return View();
        }
        public IActionResult Register()
        {
            return View();
        }
    }
}
