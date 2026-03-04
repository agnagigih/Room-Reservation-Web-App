using Microsoft.EntityFrameworkCore;
using RoomBooking.Data;
using RoomBooking.Models;

namespace RoomBooking.Services.RoomServices
{
    public class RoomService : IRoomService
    {
        private readonly AppDbContext _context;

        public RoomService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Room>> GetAllAsync(string? search)
        {
            var query = _context.Rooms.AsNoTracking().AsQueryable();

            if (!string.IsNullOrEmpty(search))
                query = query.Where(r => r.Name.Contains(search) || r.Location.Contains(search));

            return await query.OrderBy(r => r.Name).ToListAsync();
        }

        public async Task<Room?> GetByIdAsync(int id)
        {
            return await _context.Rooms.FindAsync(id);
        }

        public async Task<Room?> GetDetailAsync(int id)
        {
            return await _context.Rooms
                .AsNoTracking()
                .Include(r => r.Bookings).ThenInclude(b => b.User)
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task<bool> IsNameTakenAsync(string name, int? excludeId = null)
        {
            return await _context.Rooms
                .AnyAsync(r => r.Name == name && (excludeId == null || r.Id != excludeId));
        }

        public async Task CreateAsync(Room model)
        {
            model.CreatedAt = DateTime.Now;
            _context.Rooms.Add(model);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> UpdateAsync(int id, Room model)
        {
            var room = await _context.Rooms.FindAsync(id);
            if (room == null) return false;

            room.Name = model.Name;
            room.Location = model.Location;
            room.Capacity = model.Capacity;
            room.Description = model.Description;
            room.IsActive = model.IsActive;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeactivateAsync(int id)
        {
            var room = await _context.Rooms.FindAsync(id);
            if (room == null) return false;

            room.IsActive = false;
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
