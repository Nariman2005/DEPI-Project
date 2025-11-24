using Microsoft.EntityFrameworkCore;

namespace WebApplication1.Models
{
    public class DBContext : DbContext
    {
        public DbSet<Patient> Patients { get; set; }
        public DbSet<Doctor> Doctors { get; set; }
        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<Clinic> Clinics { get; set; }
        public DbSet<Schedule> Schedules { get; set; }
        public DbSet<VideoCallSession> VideoCallSessions { get; set; }
        public DbSet<Payment> Payments { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {

            optionsBuilder.UseSqlServer("server=NAREMAN-ADEL\\SQLEXPRESS;database=DepiDB;trusted_connection=true;TrustServerCertificate=true;");

        }

    }
}
