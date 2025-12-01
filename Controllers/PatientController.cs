using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register(Patient patient)
        {
            try
            {
                // Check if email already exists
                var existingPatient = db.Patients.FirstOrDefault(p => p.Email == patient.Email);
                if (existingPatient != null)
                {
                    ViewBag.Error = "Email already registered. Please use a different email or login.";
                    return View(patient);
                }

                // Hash password before saving
                patient.PasswordHashed = BCrypt.Net.BCrypt.HashPassword(patient.PasswordHashed);

                db.Patients.Add(patient);
                db.SaveChanges();

                ViewBag.Success = "Registration successful! Please login with your credentials.";
                return RedirectToAction("Login");
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"Registration failed: {ex.Message}";
                return View(patient);
            }
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(string email, string password)
        {
            try
            {
                // Check if parameters are received
                if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
                {
                    ViewBag.Error = "Email and password are required.";
                    return View();
                }

                var patient = db.Patients.FirstOrDefault(p => p.Email == email);

                if (patient == null)
                {
                    ViewBag.Error = "No account found with this email address.";
                    return View();
                }

                bool isPasswordValid = BCrypt.Net.BCrypt.Verify(password, patient.PasswordHashed);

                if (isPasswordValid)
                {
                    return RedirectToAction("Profile", new { id = patient.PatientId });
                }
                else
                {
                    ViewBag.Error = "Invalid password. Please try again.";
                    return View();
                }
            }
            catch (Exception ex)
            {
                ViewBag.Error = "An error occurred during login.";
                return View();
            }
        }

        [Route("Patient/Profile/{id:int}")]
        public IActionResult Profile(int id)
        {
            // Fetch patient from database
            var patient = db.Patients.Find(id);

            if (patient == null)
            {
                ViewBag.Error = "Patient not found.";
                return RedirectToAction("Login");
            }

            return View(patient);
        }

        [Route("Patient/Payment/{id:int}")]
        public IActionResult Payment(int id)
        {
            try
            {
                // Get patient with appointments, payments, and related data
                var patient = db.Patients
                    .Include(p => p.Appointments)
                        .ThenInclude(a => a.Payment)
                    .Include(p => p.Appointments)
                        .ThenInclude(a => a.Doctor)
                    .Include(p => p.Appointments)
                        .ThenInclude(a => a.Clinic)
                    .FirstOrDefault(p => p.PatientId == id);

                if (patient == null)
                {
                    return RedirectToAction("Login");
                }

                // Check if patient has appointments and create pending payments for appointments without payments
                if (patient.Appointments.Any())
                {
                    // Get appointments that don't have payments yet (excluding cancelled appointments)
                    var appointmentsWithoutPayments = patient.Appointments
                        .Where(a => a.Payment == null && a.Status != AppointmentStatus.Cancelled)
                        .ToList();

                    // Create pending payment records for appointments without payments
                    foreach (var appointment in appointmentsWithoutPayments)
                    {
                        var payment = new Payment
                        {
                            Amount = appointment.Fee,
                            Currency = "EGP", // Egyptian Pound
                            PaymentStatus = PaymentStatus.Pending, // Status = 1
                            AppointmentId = appointment.AppointmentId,
                            PaidAt = null, // Not paid yet
                            TransactionId = null // Will be set when payment is processed
                        };

                        db.Payments.Add(payment);
                    }

                    // Save changes if any new payments were created
                    if (appointmentsWithoutPayments.Any())
                    {
                        db.SaveChanges();

                        // Reload patient data to include the newly created payments
                        patient = db.Patients
                            .Include(p => p.Appointments)
                                .ThenInclude(a => a.Payment)
                            .Include(p => p.Appointments)
                                .ThenInclude(a => a.Doctor)
                            .Include(p => p.Appointments)
                                .ThenInclude(a => a.Clinic)
                            .FirstOrDefault(p => p.PatientId == id);
                    }
                }

                // Get all payments from patient's appointments with all related data
                var payments = patient.Appointments
                    .Where(a => a.Payment != null)
                    .Select(a => a.Payment)
                    .OrderByDescending(p => p.PaidAt ?? DateTime.MinValue)
                    .ThenByDescending(p => p.PaymentId)
                    .ToList();

                // Calculate summary statistics and pass via ViewBag
                ViewBag.Patient = patient;
                ViewBag.TotalPendingPayments = payments.Count(p => p.PaymentStatus == PaymentStatus.Pending);
                ViewBag.TotalCompletedPayments = payments.Count(p => p.PaymentStatus == PaymentStatus.Completed);
                ViewBag.TotalAmount = payments.Where(p => p.PaymentStatus == PaymentStatus.Completed).Sum(p => p.Amount);
                ViewBag.PendingAmount = payments.Where(p => p.PaymentStatus == PaymentStatus.Pending).Sum(p => p.Amount);

                return View(payments);
            }
            catch (Exception ex)
            {
                // Log the exception for debugging
                ViewBag.Error = $"An error occurred while loading payment data: {ex.Message}";
                return RedirectToAction("Profile", new { id = id });
            }
        }

        [Route("Patient/AppointmentHistory/{id:int}")]
        public IActionResult AppointmentHistory(int id)
        {
            var patient = db.Patients.Find(id);
            if (patient == null)
            {
                return RedirectToAction("Login");
            }

            // Get patient's past appointments with related data
            var appointmentHistory = db.Appointments
                .Include(a => a.Doctor)
                .Include(a => a.Clinic)
                .Include(a => a.Payment)
                .Where(a => a.PatientId == id)
                .Where(a => a.AppointmentDate < DateTime.Now)
                .OrderByDescending(a => a.AppointmentDate)
                .ThenByDescending(a => a.AppointmentTime)
                .ToList();

            ViewBag.Patient = patient;
            ViewBag.TotalAppointments = appointmentHistory.Count;
            ViewBag.CompletedAppointments = appointmentHistory.Count(a => a.Status == AppointmentStatus.Completed);
            ViewBag.CancelledAppointments = appointmentHistory.Count(a => a.Status == AppointmentStatus.Cancelled);

            return View(appointmentHistory);
        }

     


        // NEW: Upcoming Appointments Action
        [Route("Patient/UpcomingAppointments/{id:int}")]
        public IActionResult UpcomingAppointments(int id)
        {
            var patient = db.Patients.Find(id);
            if (patient == null)
            {
                return RedirectToAction("Login");
            }

            var now = DateTime.Now;
            var today = DateTime.Today;

            // Find patient's upcoming appointments
            var upcomingAppointments = db.Appointments
                .Include(a => a.Doctor)
                .Include(a => a.Clinic)
                .Where(a => a.PatientId == id)
                .Where(a => a.AppointmentDate > now || (a.AppointmentDate.Date == today && a.AppointmentDate.TimeOfDay >= now.TimeOfDay))
                .OrderBy(a => a.AppointmentDate)
                .ThenBy(a => a.AppointmentTime)
                .ToList();
            ViewBag.Patient = patient;
            return View(upcomingAppointments);
        }

        // NEW: Profile Info Action
        [Route("Patient/ProfileInfo/{id:int}")]
        public IActionResult ProfileInfo(int id)
        {
            var patient = db.Patients.Find(id);
            if (patient == null)
            {
                return RedirectToAction("Login");
            }

            var now = DateTime.Now;
            var today = DateTime.Today;

            var upcomingCount = db.Appointments
               .Where(a => a.PatientId == id)
               .Where(a => a.AppointmentDate > today || (a.AppointmentDate == today && a.AppointmentTime >= now.TimeOfDay))
               .Count();

            var pastCount = db.Appointments
                .Where(a => a.PatientId == id)
                .Where(a => a.AppointmentDate < today || (a.AppointmentDate == today && a.AppointmentTime < now.TimeOfDay))
                .Count();

            var nextAppointment = db.Appointments
                .Include(a => a.Doctor)
                .Include(a => a.Clinic)
                .Where(a => a.PatientId == id)
                .Where(a => a.AppointmentDate > today || (a.AppointmentDate == today && a.AppointmentTime >= now.TimeOfDay))
                .OrderBy(a => a.AppointmentDate)
                .ThenBy(a => a.AppointmentTime)
                .FirstOrDefault();

            var paymentsQuery = db.Payments
             .Join(db.Appointments, p => p.AppointmentId, a => a.AppointmentId, (p, a) => new { Payment = p, Appointment = a })
             .Where(pa => pa.Appointment.PatientId == id)
             .Select(pa => pa.Payment);

            var lastPayment = paymentsQuery
                .OrderByDescending(p => p.PaidAt)
                .FirstOrDefault();

            var totalPaid = paymentsQuery
                .Where(p => p.PaidAt != null)
                .Select(p => (decimal?)p.Amount)
                .Sum() ?? 0m;

            ViewBag.UpcomingCount = upcomingCount;
            ViewBag.PastCount = pastCount;
            ViewBag.NextAppointment = nextAppointment;
            ViewBag.LastPayment = lastPayment;
            ViewBag.TotalPaid = totalPaid;

            return View(patient);
        }


        // 1. GET: Show the Edit Form
        [HttpGet]
        [Route("Patient/EditProfile/{id:int}")]
        public IActionResult EditProfile(int id)
        {
            var patient = db.Patients.Find(id);
            if (patient == null)
            {
                return RedirectToAction("Login");
            }
            return View(patient);
        }

        // 2. POST: Process the Update
        [HttpPost]
        [Route("Patient/EditProfile/{id:int}")]
        public IActionResult EditProfile(Patient model)
        {
            try
            {
                var existingPatient = db.Patients.Find(model.PatientId);

                if (existingPatient == null)
                {
                    return RedirectToAction("Login");
                }

                // Update only the editable fields
                existingPatient.FirstName = model.FirstName;
                existingPatient.LastName = model.LastName;
                existingPatient.PhoneNumber = model.PhoneNumber;
                existingPatient.Age = model.Age;
                existingPatient.Gender = model.Gender;
                existingPatient.BloodGroup = model.BloodGroup;
                existingPatient.Height = model.Height;
                existingPatient.Weight = model.Weight;
                existingPatient.Allergies = model.Allergies;
                existingPatient.Address = model.Address;


                db.SaveChanges();

                ViewBag.Success = "Profile updated successfully!";
                return RedirectToAction("ProfileInfo", new { id = model.PatientId });
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Error updating profile: " + ex.Message;
                return View(model);
            }
        }
    }
}