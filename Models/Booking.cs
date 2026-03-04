using System.ComponentModel.DataAnnotations;

namespace RoomBooking.Models
{
    public class Booking
    {
        public int Id { get; set; }

        public int UserId { get; set; }
        public User? User { get; set; }

        public int RoomId { get; set; }
        public Room? Room { get; set; }

        [Required]
        public DateTime BookingDate { get; set; }

        [Required, MaxLength(5)]
        public string StartTime { get; set; } = string.Empty; // e.g. "08:00"

        [Required, MaxLength(5)]
        public string EndTime { get; set; } = string.Empty; // e.g. "10:00"

        [Required, MaxLength(200)]
        public string Purpose { get; set; } = string.Empty;

        [MaxLength(20)]
        public string Status { get; set; } = "Pending"; // Pending, Approved, Rejected, Cancelled

        [MaxLength(500)]
        public string? Notes { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime? UpdatedAt { get; set; }
    }
}
