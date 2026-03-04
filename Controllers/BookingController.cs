using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RoomBooking.Data;
using RoomBooking.Services.BookingServices;
using RoomBooking.ViewModels;
using System.Security.Claims;

namespace RoomBooking.Controllers
{
    [Authorize]
    public class BookingController : Controller
    {
        private readonly IBookingService _bookingService;

        public BookingController(IBookingService bookingService)
        {
            _bookingService = bookingService;
        }
        // pulled from cookie claims
        private int CurrentUserId =>
            int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        private bool IsAdmin => User.IsInRole("Admin");

        public async Task<IActionResult> Index(DateTime? filterDate, int? filterRoomId, string? filterStatus)
        {
            var vm = await _bookingService.GetFilteredAsync(
                filterDate, filterRoomId, filterStatus,
                CurrentUserId, IsAdmin);

            return View(vm);
        }

        public async Task<IActionResult> Create()
        {
            return View(new BookingCreateViewModel
            {
                AvailableRooms = await _bookingService.GetActiveRoomsAsync()
            });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BookingCreateViewModel model)
        {
            model.AvailableRooms = await _bookingService.GetActiveRoomsAsync();

            if (!ModelState.IsValid) return View(model);

            if (string.Compare(model.StartTime, model.EndTime) >= 0)
            {
                ModelState.AddModelError("EndTime", "Jam selesai harus lebih dari jam mulai.");
                return View(model);
            }

            if (await _bookingService.HasConflictAsync(model.RoomId, model.BookingDate, model.StartTime, model.EndTime))
            {
                ModelState.AddModelError("", "Ruangan sudah dibooking pada jam tersebut. Silakan pilih waktu lain.");
                return View(model);
            }

            await _bookingService.CreateAsync(model, CurrentUserId, IsAdmin);
            TempData["Success"] = IsAdmin
                ? "Booking berhasil dibuat dan langsung disetujui."
                : "Booking berhasil dibuat. Menunggu persetujuan admin.";

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var booking = await _bookingService.GetByIdAsync(id);
            if (booking == null) return NotFound();
            if (!IsAdmin && booking.UserId != CurrentUserId) return Forbid();
            if (booking.Status == "Approved" && !IsAdmin)
            {
                TempData["Error"] = "Booking yang sudah disetujui tidak dapat diedit.";
                return RedirectToAction(nameof(Index));
            }

            return View(new BookingEditViewModel
            {
                Id = booking.Id,
                RoomId = booking.RoomId,
                BookingDate = booking.BookingDate,
                StartTime = booking.StartTime,
                EndTime = booking.EndTime,
                Purpose = booking.Purpose,
                Notes = booking.Notes,
                Status = booking.Status,
                AvailableRooms = await _bookingService.GetActiveRoomsAsync()
            });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, BookingEditViewModel model)
        {
            model.AvailableRooms = await _bookingService.GetActiveRoomsAsync();

            if (id != model.Id) return BadRequest();
            if (!ModelState.IsValid) return View(model);

            var booking = await _bookingService.GetByIdAsync(id);
            if (booking == null) return NotFound();
            if (!IsAdmin && booking.UserId != CurrentUserId) return Forbid();

            if (string.Compare(model.StartTime, model.EndTime) >= 0)
            {
                ModelState.AddModelError("EndTime", "Jam selesai harus lebih dari jam mulai.");
                return View(model);
            }

            if (await _bookingService.HasConflictAsync(model.RoomId, model.BookingDate, model.StartTime, model.EndTime, excludeBookingId: id))
            {
                ModelState.AddModelError("", "Ruangan sudah dibooking pada jam tersebut.");
                return View(model);
            }

            await _bookingService.UpdateAsync(id, model, IsAdmin);
            TempData["Success"] = "Booking berhasil diperbarui.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Detail(int id)
        {
            var booking = await _bookingService.GetDetailAsync(id);
            if (booking == null) return NotFound();
            if (!IsAdmin && booking.UserId != CurrentUserId) return Forbid();
            return View(booking);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancel(int id)
        {
            var booking = await _bookingService.GetByIdAsync(id);
            if (booking == null) return NotFound();
            if (!IsAdmin && booking.UserId != CurrentUserId) return Forbid();

            await _bookingService.ChangeStatusAsync(id, "Cancelled");
            TempData["Success"] = "Booking berhasil dibatalkan.";
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin")]
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Approve(int id)
        {
            if (!await _bookingService.ChangeStatusAsync(id, "Approved")) return NotFound();
            TempData["Success"] = "Booking berhasil disetujui.";
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin")]
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Reject(int id)
        {
            if (!await _bookingService.ChangeStatusAsync(id, "Rejected")) return NotFound();
            TempData["Success"] = "Booking berhasil ditolak.";
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin")]
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            if (!await _bookingService.DeleteAsync(id)) return NotFound();
            TempData["Success"] = "Booking berhasil dihapus.";
            return RedirectToAction(nameof(Index));
        }
    }
}
