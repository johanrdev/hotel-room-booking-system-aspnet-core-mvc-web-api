using Microsoft.AspNetCore.Identity;

namespace HotelBookingSystem.API.Models {
    public class Booking
    {
        public int Id { get; set; }
        public int RoomId { get; set; }
        public string UserId { get; set; } = string.Empty;
        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }

        public Room? Room { get; set; }
        public IdentityUser? User { get; set; }
    }
}