using RoomBooking.Models;

namespace RoomBooking.Services.RoomServices
{
    public interface IRoomService
    {
        public Task<List<Room>> GetAllAsync(string? search);
        public Task<Room?> GetByIdAsync(int id);
        public Task<Room?> GetDetailAsync(int id);         // includes Bookings + User
        public Task<bool> IsNameTakenAsync(string name, int? excludeId = null);
        public Task CreateAsync(Room model);
        public Task<bool> UpdateAsync(int id, Room model);
        public Task<bool> DeactivateAsync(int id);
    }
}
