using RoomBooking.Models;
using RoomBooking.ViewModels;

namespace RoomBooking.Services.UserServices
{
    public interface IUserService
    {
        public Task<List<User>> GetAllAsync(string? search);
        public Task<User?> GetByIdAsync(int id);
        public Task<User?> GetDetailAsync(int id);        // includes Bookings + Room
        public Task<bool> IsEmailTakenAsync(string email, int? excludeId = null);
        public Task CreateAsync(UserCreateViewModel model);
        public Task<bool> UpdateAsync(int id, UserEditViewModel model);
        public Task<bool> DeleteAsync(int id);
        public Task<bool> DeactivateAsync(int id);
    }
}
