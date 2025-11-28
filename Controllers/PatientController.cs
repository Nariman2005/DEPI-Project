using Microsoft.AspNetCore.Mvc;
using WebApplication1.Models;


namespace WebApplication1.Controllers
{
    public class PatientController : Controller
    {
        DBContext db;

        public PatientController()
        {
            db = new DBContext();
        }
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Login(string email, string password)
        {
           
            var patient = db.Patients.FirstOrDefault(p => p.Email == email);

            if (patient == null)
            {
              
                return View();
            }
           
            bool isPasswordValid = BCrypt.Net.BCrypt.Verify(password, patient.PasswordHashed);

            if (isPasswordValid)
            {
                
                return RedirectToAction("Profile");
            }
            else
            {
                ViewBag.Error = "Invalid email or password.";
                return View();
            }
        }



        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Register(Patient patient)
        {
            // Check if email already exists
            var existingPatient = db.Patients.FirstOrDefault(p => p.Email == patient.Email);
            if (existingPatient != null)
            { 
                return View();
            }

            // Hash password before saving
            patient.PasswordHashed = BCrypt.Net.BCrypt.HashPassword(patient.PasswordHashed);

            db.Patients.Add(patient);
            db.SaveChanges();

            
            return RedirectToAction("Login");

   
        }
    }
}

