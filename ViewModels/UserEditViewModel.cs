using System.ComponentModel.DataAnnotations;

namespace RoomBooking.ViewModels
{
    public class UserEditViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Nama lengkap wajib diisi")]
        [MaxLength(100)]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email wajib diisi")]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Role { get; set; } = "User";

        public bool IsActive { get; set; } = true;

        [DataType(DataType.Password)]
        [MinLength(6, ErrorMessage = "Password minimal 6 karakter")]
        public string? NewPassword { get; set; }
    }
}
