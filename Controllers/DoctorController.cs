using Microsoft.AspNetCore.Mvc;

namespace WebApplication1.Controllers
{
    public class DoctorController : Controller
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
