using RoomBooking.Models;
using System.Security.Claims;

namespace RoomBooking.Services.AuthServices
{
    public interface IAuthService
    {
        public Task<User?> ValidateUserAsync(string email, string password);
        public ClaimsPrincipal BuildClaimsPrincipal(User user);
    }
}
