using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RoomBooking.Data;
using RoomBooking.Models;
using RoomBooking.Services.RoomServices;

namespace RoomBooking.Controllers
{
    [Authorize]
    public class RoomController : Controller
    {
        private readonly IRoomService _roomService;

        public RoomController(IRoomService roomService)
        {
            _roomService = roomService;
        }

        public async Task<IActionResult> Index(string? search)
        {
            ViewBag.Search = search;
            return View(await _roomService.GetAllAsync(search));
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Create() => View(new Room());

        [Authorize(Roles = "Admin")]
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Room model)
        {
            if (!ModelState.IsValid) return View(model);

            if (await _roomService.IsNameTakenAsync(model.Name))
            {
                ModelState.AddModelError("Name", "Nama ruangan sudah ada.");
                return View(model);
            }

            await _roomService.CreateAsync(model);
            TempData["Success"] = $"Ruangan '{model.Name}' berhasil ditambahkan.";
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id)
        {
            var room = await _roomService.GetByIdAsync(id);
            if (room == null) return NotFound();
            return View(room);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Room model)
        {
            if (id != model.Id) return BadRequest();
            if (!ModelState.IsValid) return View(model);

            if (await _roomService.IsNameTakenAsync(model.Name, excludeId: id))
            {
                ModelState.AddModelError("Name", "Nama ruangan sudah dipakai.");
                return View(model);
            }

            var success = await _roomService.UpdateAsync(id, model);
            if (!success) return NotFound();

            TempData["Success"] = $"Ruangan '{model.Name}' berhasil diperbarui.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Detail(int id)
        {
            var room = await _roomService.GetDetailAsync(id);
            if (room == null) return NotFound();
            return View(room);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var room = await _roomService.GetByIdAsync(id);
            if (room == null) return NotFound();

            await _roomService.DeactivateAsync(id);
            TempData["Success"] = $"Ruangan '{room.Name}' berhasil dinonaktifkan.";
            return RedirectToAction(nameof(Index));
        }
    }
}