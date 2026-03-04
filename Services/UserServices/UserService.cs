// Services/User/UserService.cs
using Microsoft.EntityFrameworkCore;
using RoomBooking.Data;
using RoomBooking.Models;
using RoomBooking.ViewModels;

namespace RoomBooking.Services.UserServices
{
    public class UserService : IUserService
    {
        private readonly AppDbContext _context;

        public UserService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<User>> GetAllAsync(string? search)
        {
            var query = _context.Users.AsNoTracking().AsQueryable();

            if (!string.IsNullOrEmpty(search))
                query = query.Where(u => u.FullName.Contains(search) || u.Email.Contains(search))
                    .Where(u => u.IsDeleted == false);

            return await query.OrderBy(u => u.FullName).ToListAsync();
        }

        public async Task<User?> GetByIdAsync(int id)
        {
            return await _context.Users.Where(u => u.Id == id && u.IsDeleted == false).FirstOrDefaultAsync();
        }

        public async Task<User?> GetDetailAsync(int id)
        {
            return await _context.Users
                .AsNoTracking()
                .Include(u => u.Bookings).ThenInclude(b => b.Room)
                .Where (u => u.IsDeleted == false)
                .FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<bool> IsEmailTakenAsync(string email, int? excludeId = null)
        {
            return await _context.Users
                .Where(u=> u.IsDeleted == false)
                .AnyAsync(u => u.Email == email && (excludeId == null || u.Id != excludeId));
        }

        public async Task CreateAsync(UserCreateViewModel model)
        {
            var user = new User
            {
                FullName = model.FullName,
                Email = model.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.Password),
                Role = model.Role,
                IsActive = true,
                IsDeleted = false,
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> UpdateAsync(int id, UserEditViewModel model)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return false;

            user.FullName = model.FullName;
            user.Email = model.Email;
            user.Role = model.Role;
            user.IsActive = model.IsActive;

            if (!string.IsNullOrEmpty(model.NewPassword))
                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.NewPassword);

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return false;

            // soft delete
            user.IsDeleted = false;
            await _context.SaveChangesAsync();
            return true;
        }
    }
}