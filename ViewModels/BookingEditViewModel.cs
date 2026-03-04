using RoomBooking.Models;
using System.ComponentModel.DataAnnotations;

namespace RoomBooking.ViewModels
{
    public class BookingEditViewModel
    {
        public int Id { get; set; }

        [Required]
        public int RoomId { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime BookingDate { get; set; }

        [Required]
        public string StartTime { get; set; } = string.Empty;

        [Required]
        public string EndTime { get; set; } = string.Empty;

        [Required]
        [MaxLength(200)]
        public string Purpose { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Notes { get; set; }

        public string Status { get; set; } = "Pending";

        public List<Room> AvailableRooms { get; set; } = new();
    }
}
