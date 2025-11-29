//using Microsoft.AspNetCore.Mvc;
//using WebApplication1.Models;

//namespace WebApplication1.Controllers
//{
//    public class DoctorController : Controller
//    {
//        DBContext db;

//        public DoctorController()
//        {
//            db = new DBContext();
//        }

//        [HttpGet]
//        public IActionResult Register()
//        {
//            return View();
//        }

//        [HttpPost]
//        public IActionResult Register(Doctor Doctor)
//        {
//            try
//            {
//                // Check if email already exists
//                var existingPatient = db.Patients.FirstOrDefault(p => p.Email == Doctor.Email);
//                if (existingPatient != null)
//                {
//                    ViewBag.Error = "Email already registered. Please use a different email or login.";
//                    return View(Doctor);
//                }

//                // Hash password before saving
//                Doctor.PasswordHashed = BCrypt.Net.BCrypt.HashPassword(Doctor.PasswordHashed);

//                db.Patients.Add(Doctor);
//                db.SaveChanges();

//                ViewBag.Success = "Registration successful! Please login with your credentials.";
//                return RedirectToAction("Login");
//            }
//            catch (Exception ex)
//            {
//                ViewBag.Error = $"Registration failed: {ex.Message}";
//                return View(Doctor);
//            }
//        }

//        [HttpGet]
//        public IActionResult Login()
//        {
//            return View();
//        }

//        [HttpPost]
//        public IActionResult Login(string email, string password)
//        {
//            try
//            {


//                // Check if parameters are received
//                if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
//                {
//                    ViewBag.Error = "Email and password are required.";
//                    return View();
//                }

//                var patient = db.Doctor.FirstOrDefault(p => p.Email == email);

//                if (patient == null)
//                {
//                    ViewBag.Error = "No account found with this email address.";
//                    return View();
//                }


//                System.Diagnostics.Debug.WriteLine($"Found patient: {patient.FirstName}");

//                bool isPasswordValid = BCrypt.Net.BCrypt.Verify(password, patient.PasswordHashed);


//                if (isPasswordValid)
//                {
//                    //TempData["PatientId"] = patient.PatientId;
//                    return RedirectToAction("Doctor", new { id = Doctor.DoctorId });
//                }
//                else
//                {
//                    ViewBag.Error = "Invalid password. Please try again.";
//                    return View();
//                }
//            }
//            catch (Exception ex)
//            {

//                return View();
//            }
//        }

//        [Route("Doctor/Profile/{id:int}")]
//        public IActionResult Profile(int id)
//        {

//            // Fetch patient from database
//            var Doctor = db.Doctors.Find(id);

//            if (Doctor == null)
//            {
//                ViewBag.Error = "Patient not found.";
//                return RedirectToAction("Login");
//            }

//            return View(Doctor);
//        }


//    }
//}
