using System.Security.Claims;
using HotelBookingSystem.API.Data;
using HotelBookingSystem.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HotelBookingSystem.API.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    public class BookingController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ILogger<BookingController> _logger;

        public BookingController(AppDbContext context, ILogger<BookingController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Booking>>> GetBookings()
        {
            return await _context.Bookings.Include(b => b.Room).Include(b => b.User).ToListAsync();
        }

        [Authorize(Roles = "Admin,Guest")]
        [HttpGet("{id}")]
        public async Task<ActionResult<Booking>> GetBooking(int id)
        {
            var booking = await _context.Bookings.Include(b => b.Room).Include(b => b.User).FirstOrDefaultAsync(b => b.Id == id);

            if (booking == null)
            {
                return NotFound();
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var isAdmin = User.IsInRole("Admin");
            var isOwner = booking.UserId == userId;

            if (!isAdmin && !isOwner)
            {
                return Forbid();
            }

            return booking;
        }

        [Authorize(Roles = "Admin,Guest")]
        [HttpPost]
        public async Task<ActionResult<Booking>> PostBooking(BookingCreateDTO bookingCreateDTO)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            _logger.LogInformation($"User ID is set to: {userId}");

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User is not authenticated.");
            }

            var availableRoom = await _context.Rooms
                .Where(room => room.Type == bookingCreateDTO.RoomType)
                .Where(room => !_context.Bookings.Any(b =>
                    b.RoomId == room.Id &&
                    b.CheckInDate < bookingCreateDTO.CheckOutDate &&
                    b.CheckOutDate > bookingCreateDTO.CheckInDate))
                .FirstOrDefaultAsync();

            if (availableRoom == null)
            {
                return BadRequest("No available rooms for the selected type and dates.");
            }

            var booking = new Booking
            {
                RoomId = availableRoom.Id,
                CheckInDate = bookingCreateDTO.CheckInDate,
                CheckOutDate = bookingCreateDTO.CheckOutDate,
                UserId = userId
            };

            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetBooking", new { id = booking.Id }, booking);
        }

        [Authorize(Roles = "Admin,Guest")]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutBooking(int id, Booking booking)
        {
            if (id != booking.Id)
            {
                return BadRequest();
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var isAdmin = User.IsInRole("Admin");
            var isOwner = booking.UserId == userId;

            if (!isAdmin && !isOwner)
            {
                return Forbid();
            }

            _context.Entry(booking).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BookingExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        [Authorize(Roles = "Admin,Guest")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBooking(int id)
        {
            var booking = await _context.Bookings.FindAsync(id);
            if (booking == null)
            {
                return NotFound();
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var isAdmin = User.IsInRole("Admin");
            var isOwner = booking.UserId == userId;

            if (!isAdmin && !isOwner)
            {
                return Forbid();
            }

            _context.Bookings.Remove(booking);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [Authorize(Roles = "Admin,Guest")]
        [HttpGet("user")]
        public async Task<ActionResult<IEnumerable<Booking>>> GetUserBookings()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User is not authenticated.");
            }

            var bookings = await _context.Bookings
                .Include(b => b.Room)
                .Where(b => b.UserId == userId)
                .ToListAsync();

            return Ok(bookings);
        }

        private bool BookingExists(int id)
        {
            return _context.Bookings.Any(e => e.Id == id);
        }
    }

    public class BookingCreateDTO
    {
        public string RoomType { get; set; } = string.Empty;
        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }
    }
}
