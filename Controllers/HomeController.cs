using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RoomBooking.Data;

namespace RoomBooking.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly AppDbContext _db;

        public HomeController(AppDbContext db) { _db = db; }

        public async Task<IActionResult> Index()
        {
            ViewBag.TotalRooms = await _db.Rooms.CountAsync(r => r.IsActive);
            ViewBag.TotalUsers = await _db.Users.CountAsync(u => u.IsActive);
            ViewBag.TotalBookings = await _db.Bookings.CountAsync();
            ViewBag.TodayBookings = await _db.Bookings
                .CountAsync(b => b.BookingDate.Date == DateTime.Today);
            ViewBag.PendingBookings = await _db.Bookings
                .CountAsync(b => b.Status == "Pending");
            ViewBag.RecentBookings = await _db.Bookings
                .Include(b => b.User)
                .Include(b => b.Room)
                .OrderByDescending(b => b.CreatedAt)
                .Take(5)
                .ToListAsync();
            return View();
        }
    }
}
