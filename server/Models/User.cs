using Microsoft.AspNetCore.Identity;

namespace HotelBookingSystem.API.Models {
    public class User : IdentityUser
    {
        public string Password { get; set; } = string.Empty;
    }
}