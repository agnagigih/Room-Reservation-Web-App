using RoomBooking.Models;
using RoomBooking.ViewModels;

namespace RoomBooking.Services.BookingServices
{
    public interface IBookingService
    {
        public Task<BookingFilterViewModel> GetFilteredAsync(
            DateTime? filterDate, int? filterRoomId, string? filterStatus,
            int currentUserId, bool isAdmin);

        public Task<List<Room>> GetActiveRoomsAsync();

        public Task<Booking?> GetByIdAsync(int id);

        public Task<Booking?> GetDetailAsync(int id);    // includes User + Room

        public Task<bool> HasConflictAsync(
            int roomId, DateTime date, string startTime, string endTime,
            int? excludeBookingId = null);

        public Task CreateAsync(BookingCreateViewModel model, int userId, bool isAdmin);

        public Task<bool> UpdateAsync(int id, BookingEditViewModel model, bool isAdmin);

        public Task<bool> ChangeStatusAsync(int id, string status);

        public Task<bool> DeleteAsync(int id);
    }
}
