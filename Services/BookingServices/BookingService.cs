using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using RoomBooking.Data;
using RoomBooking.Models;
using RoomBooking.ViewModels;

namespace RoomBooking.Services.BookingServices
{
    public class BookingService : IBookingService
    {
        private readonly AppDbContext _context;

        public BookingService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<BookingFilterViewModel> GetFilteredAsync(
            DateTime? filterDate, int? filterRoomId, string? filterStatus,
            int currentUserId, bool isAdmin)
        {
            var query = _context.Bookings
                .AsNoTracking()
                .Include(b => b.User)
                .Include(b => b.Room)
                .AsQueryable();

            if (!isAdmin)
                query = query.Where(b => b.UserId == currentUserId);

            if (filterDate.HasValue)
                query = query.Where(b => b.BookingDate.Date == filterDate.Value.Date);

            if (filterRoomId.HasValue)
                query = query.Where(b => b.RoomId == filterRoomId);

            if (!string.IsNullOrEmpty(filterStatus))
                query = query.Where(b => b.Status == filterStatus);

            return new BookingFilterViewModel
            {
                FilterDate = filterDate,
                FilterRoomId = filterRoomId,
                FilterStatus = filterStatus,
                Bookings = await query
                    .OrderByDescending(b => b.BookingDate)
                    .ThenBy(b => b.StartTime)
                    .ToListAsync(),
                Rooms = await GetActiveRoomsAsync()
            };
        }

        public async Task<List<Room>> GetActiveRoomsAsync()
        {
            return await _context.Rooms
                .AsNoTracking()
                .Where(r => r.IsActive)
                .OrderBy(r => r.Name)
                .ToListAsync();
        }

        public async Task<Booking?> GetByIdAsync(int id)
        {
            return await _context.Bookings.FindAsync(id);
        }

        public async Task<Booking?> GetDetailAsync(int id)
        {
            return await _context.Bookings
                .AsNoTracking()
                .Include(b => b.User)
                .Include(b => b.Room)
                .FirstOrDefaultAsync(b => b.Id == id);
        }

        public async Task<bool> HasConflictAsync(
            int roomId, DateTime date, string startTime, string endTime,
            int? excludeBookingId = null)
        {
            // ================================================================================================
            // comment dan uncomment salah satu
            // ================================================================================================

            // ================================================================================================
            // jika menggunakan NuGet Package Manager → Package Manager Console Add-Migration & Update-Database
            // ================================================================================================
            return await _context.Bookings.AnyAsync(b =>
                b.RoomId == roomId &&
                b.BookingDate.Date == date.Date &&
                b.Status != "Cancelled" && b.Status != "Rejected" &&
                (excludeBookingId == null || b.Id != excludeBookingId) &&
                b.StartTime.CompareTo(endTime) < 0 &&
                b.EndTime.CompareTo(startTime) > 0);

            // ================================================================================================
            // jika menggunakan Script SQL Manual (Tanpa Migration) - Database_Script.sql
            // ================================================================================================
            //var result = await _context.Database
            //    .SqlQueryRaw<int>(
            //    "EXEC sp_CekKetersediaanRuangan @RoomId, @Tanggal, @JamMulai, @JamSelesai, @ExcludeBookingId",
            //    new SqlParameter("@RoomId", roomId),
            //    new SqlParameter("@Tanggal", date.Date),
            //    new SqlParameter("@JamMulai", startTime),
            //    new SqlParameter("@JamSelesai", endTime),
            //    new SqlParameter("@ExcludeBookingId", (object?)excludeBookingId ?? DBNull.Value)
            //    )
            //    .FirstAsync();

            //return result > 0;
        }

        public async Task CreateAsync(BookingCreateViewModel model, int userId, bool isAdmin)
        {
            var booking = new Booking
            {
                UserId = userId,
                RoomId = model.RoomId,
                BookingDate = model.BookingDate,
                StartTime = model.StartTime,
                EndTime = model.EndTime,
                Purpose = model.Purpose,
                Notes = model.Notes,
                Status = isAdmin ? "Approved" : "Pending"
            };

            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> UpdateAsync(int id, BookingEditViewModel model, bool isAdmin)
        {
            var booking = await _context.Bookings.FindAsync(id);
            if (booking == null) return false;

            booking.RoomId = model.RoomId;
            booking.BookingDate = model.BookingDate;
            booking.StartTime = model.StartTime;
            booking.EndTime = model.EndTime;
            booking.Purpose = model.Purpose;
            booking.Notes = model.Notes;
            booking.UpdatedAt = DateTime.Now;

            if (isAdmin) booking.Status = model.Status;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ChangeStatusAsync(int id, string status)
        {
            var booking = await _context.Bookings.FindAsync(id);
            if (booking == null) return false;

            booking.Status = status;
            booking.UpdatedAt = DateTime.Now;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var booking = await _context.Bookings.FindAsync(id);
            if (booking == null) return false;

            _context.Bookings.Remove(booking);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
