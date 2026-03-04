using RoomBooking.Models;
using System.ComponentModel.DataAnnotations;

namespace RoomBooking.ViewModels
{
    public class BookingFilterViewModel
    {
        public DateTime? FilterDate { get; set; }
        public int? FilterRoomId { get; set; }
        public string? FilterStatus { get; set; }
        public List<Booking> Bookings { get; set; } = new();
        public List<Room> Rooms { get; set; } = new();
    }
}
