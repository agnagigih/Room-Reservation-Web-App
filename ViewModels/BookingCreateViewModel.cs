using RoomBooking.Models;
using System.ComponentModel.DataAnnotations;

namespace RoomBooking.ViewModels
{
    public class BookingCreateViewModel
    {
        [Required(ErrorMessage = "Pilih ruangan")]
        public int RoomId { get; set; }

        [Required(ErrorMessage = "Tanggal booking wajib diisi")]
        [DataType(DataType.Date)]
        public DateTime BookingDate { get; set; } = DateTime.Today;

        [Required(ErrorMessage = "Jam mulai wajib diisi")]
        public string StartTime { get; set; } = "08:00";

        [Required(ErrorMessage = "Jam selesai wajib diisi")]
        public string EndTime { get; set; } = "09:00";

        [Required(ErrorMessage = "Keperluan wajib diisi")]
        [MaxLength(200)]
        public string Purpose { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Notes { get; set; }

        public List<Room> AvailableRooms { get; set; } = new();
    }
}
