using System.ComponentModel.DataAnnotations;


namespace WebApplication1.Models
{
    public class VideoCallSession
    {
        [Key]
        public int SessionId { get; set; }

        [Required]
        public int AppointmentId { get; set; }

        [Required]
        public DateTime StartTime { get; set; }

        public DateTime? EndTime { get; set; }

        [Required]
        public VideoCallStatus Status { get; set; }

        [Required]
        public SessionType SessionType { get; set; }

        [MaxLength(500)]
        public string? VideoCallUrl { get; set; }


        public DateTime? SessionTime { get; set; }

  

    
        public  Appointment Appointment { get; set; }
    }

    public enum VideoCallStatus
    {
        Scheduled = 1,
        Pending=2,
        InProgress = 3,
        Completed = 4,
        Cancelled = 5
        
    }

    public enum SessionType
    {
        Consultation = 1,
        FollowUp = 2,
        Emergency = 3
    }
}