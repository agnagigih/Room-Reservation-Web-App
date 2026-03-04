using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RoomBooking.Data;
using RoomBooking.Services.UserServices;
using RoomBooking.ViewModels;

namespace RoomBooking.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UserController : Controller
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        public async Task<IActionResult> Index(string? search)
        {
            ViewBag.Search = search;
            var users = await _userService.GetAllAsync(search);
            return View(users);
        }

        public IActionResult Create() => View(new UserCreateViewModel());

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UserCreateViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            if (await _userService.IsEmailTakenAsync(model.Email))
            {
                ModelState.AddModelError("Email", "Email sudah terdaftar.");
                return View(model);
            }

            
            await _userService.CreateAsync(model);
            TempData["Success"] = $"User '{model.FullName}' berhasil ditambahkan.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var user = await _userService.GetByIdAsync(id);
            if (user == null) return NotFound();

            var viewUser = new UserEditViewModel
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                Role = user.Role,
                IsActive = user.IsActive
            };
            return View(viewUser);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, UserEditViewModel model)
        {
            if (id != model.Id) return BadRequest();
            if (!ModelState.IsValid) return View(model);

            if (await _userService.IsEmailTakenAsync(model.Email, excludeId: id))
            {
                ModelState.AddModelError("Email", "Email sudah digunakan user lain.");
                return View(model);
            }

            var success = await _userService.UpdateAsync(id, model);
            if(!success) return NotFound();
            
            TempData["Success"] = $"User '{model.FullName}' berhasil diperbarui.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Detail(int id)
        {
            var user = await _userService.GetByIdAsync(id);
            if (user == null) return NotFound();
            return View(user);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var user = await _userService.GetByIdAsync(id);
            if (user == null) return NotFound();

            await _userService.DeleteAsync(id);
            TempData["Success"] = $"User '{user.FullName}' berhasil dihapus.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Deactivate(int id)
        {
            var user = await _userService.GetByIdAsync(id);
            if (user == null) return NotFound();

            await _userService.DeactivateAsync(id);
            TempData["Success"] = $"User '{user.FullName}' berhasil dinonaktifkan.";
            return RedirectToAction(nameof(Index));
        }
    }
}
