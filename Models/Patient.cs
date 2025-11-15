using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models
{
    public class Patient
    {
        // by convention , dataanotation, fluent api


        [Key]
        public int Patient_Id { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PasswordHashed { get; set; }
        public string PhoneNumber { get; set; }
        public string BloodGroup { get; set; }
        public decimal Height { get; set; }
        public decimal Weight { get; set; }
        public string Allergies { get; set; }
        public string Address { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Gender { get; set; }
        public DateTime DateOfBirth { get; set; }
    }
}

