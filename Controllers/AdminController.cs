using Microsoft.AspNetCore.Mvc;

namespace WebApplication1.Controllers
{
    public class AdminController : Controller
    {
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(string username, string password)
        {
            // Check if both username and password are "admin123"
            if (username == "admin123" && password == "admin123")
            {
                // Login successful - redirect to Dashboard
                return RedirectToAction("Dashboard");
            }
            else
            {
                // Login failed - return to login with error message
                ViewBag.Error = "Invalid username or password. Please try again.";
                return View();
            }
        }

        public IActionResult Dashboard()
        {
            return View();
        }
    }
}