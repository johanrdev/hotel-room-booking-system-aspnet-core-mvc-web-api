using System.Security.Claims;
using HotelBookingSystem.API.Data;
using HotelBookingSystem.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HotelBookingSystem.API.Controllers
{
    [Authorize(Roles = "Admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public AdminController(AppDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpGet("dashboard")]
        public async Task<ActionResult<AdminDashboardDataDTO>> GetDashboardData()
        {
            var totalUsers = await _context.Users.CountAsync();
            var totalBookings = await _context.Bookings.CountAsync();
            var totalRooms = await _context.Rooms.CountAsync();
            var totalRoomTypes = await _context.RoomTypes.CountAsync();

            var data = new AdminDashboardDataDTO
            {
                TotalUsers = totalUsers,
                TotalBookings = totalBookings,
                TotalRooms = totalRooms,
                TotalRoomTypes = totalRoomTypes
            };

            return Ok(data);
        }

        [HttpGet("users")]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            var users = await _context.Users.ToListAsync();
            return Ok(users);
        }

        [HttpPost("users")]
        public async Task<ActionResult<User>> CreateUser(AdminUserCreateDTO userCreateDTO)
        {
            var user = new User
            {
                UserName = userCreateDTO.UserName,
                Email = userCreateDTO.Email
            };

            var result = await _userManager.CreateAsync(user, userCreateDTO.Password);

            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            var roleResult = await _userManager.AddToRoleAsync(user, "Guest");

            if (!roleResult.Succeeded)
            {
                return BadRequest(roleResult.Errors);
            }

            return CreatedAtAction(nameof(GetUsers), new { id = user.Id }, user);
        }

        [HttpPut("users/{id}")]
        public async Task<IActionResult> UpdateUser(string id, AdminUserUpdateDTO userUpdateDTO)
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            user.UserName = userUpdateDTO.UserName;
            user.Email = userUpdateDTO.Email;

            _context.Entry(user).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                return BadRequest("Failed to update user.");
            }

            return Ok(user);
        }

        [HttpDelete("users/{id}")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            _context.Users.Remove(user);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                return BadRequest("Failed to delete user.");
            }

            return NoContent();
        }

        [HttpPost("bookings")]
        public async Task<ActionResult<Booking>> CreateBooking(AdminBookingCreateDTO bookingCreateDTO)
        {
            var user = await _userManager.FindByIdAsync(bookingCreateDTO.UserId);
            if (user == null)
            {
                return BadRequest("User does not exist.");
            }

            var roomType = await _context.RoomTypes
                .FirstOrDefaultAsync(rt => rt.Name == bookingCreateDTO.RoomType);
            if (roomType == null)
            {
                return BadRequest("Room type does not exist.");
            }

            var availableRoom = await _context.Rooms
                .Where(room => room.RoomTypeId == roomType.Id)
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
                UserId = bookingCreateDTO.UserId
            };

            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetBookingById), new { id = booking.Id }, booking);
        }

        [HttpGet("bookings/{id}")]
        public async Task<ActionResult<Booking>> GetBookingById(int id)
        {
            var booking = await _context.Bookings.Include(b => b.Room).Include(b => b.User).FirstOrDefaultAsync(b => b.Id == id);

            if (booking == null)
            {
                return NotFound();
            }

            return Ok(booking);
        }

        [HttpGet("bookings")]
        public async Task<ActionResult<IEnumerable<Booking>>> GetBookings()
        {
            var bookings = await _context.Bookings
                .Include(b => b.Room)
                    .ThenInclude(r => r.RoomType)
                .Include(b => b.User)
                .ToListAsync();

            return Ok(bookings);
        }

        [HttpPut("bookings/{id}")]
        public async Task<IActionResult> UpdateBooking(int id, AdminBookingUpdateDTO bookingUpdateDTO)
        {
            var booking = await _context.Bookings.Include(b => b.Room).FirstOrDefaultAsync(b => b.Id == id);

            if (booking == null)
            {
                return NotFound();
            }

            var user = await _context.Users.FindAsync(bookingUpdateDTO.UserId);
            if (user == null)
            {
                return BadRequest("Invalid user ID.");
            }

            var room = await _context.Rooms.FindAsync(bookingUpdateDTO.RoomId);
            if (room == null)
            {
                return BadRequest("Invalid room ID.");
            }

            var availableRoom = await _context.Rooms
                .Where(r => r.RoomTypeId == room.RoomTypeId && r.Id == room.Id)
                .Where(r => !_context.Bookings.Any(b =>
                    b.RoomId == r.Id &&
                    b.CheckInDate < bookingUpdateDTO.CheckOutDate &&
                    b.CheckOutDate > bookingUpdateDTO.CheckInDate &&
                    b.Id != booking.Id))
                .FirstOrDefaultAsync();

            if (availableRoom == null)
            {
                return BadRequest("No available rooms for the selected type and dates.");
            }

            booking.RoomId = availableRoom.Id;
            booking.UserId = bookingUpdateDTO.UserId;
            booking.CheckInDate = bookingUpdateDTO.CheckInDate;
            booking.CheckOutDate = bookingUpdateDTO.CheckOutDate;

            _context.Entry(booking).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                return BadRequest("Failed to update booking.");
            }

            booking.User = user;
            booking.Room = availableRoom;

            return Ok(booking);
        }

        [HttpDelete("bookings/{id}")]
        public async Task<IActionResult> DeleteBooking(int id)
        {
            var booking = await _context.Bookings.FindAsync(id);

            if (booking == null)
            {
                return NotFound();
            }

            _context.Bookings.Remove(booking);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                return BadRequest("Failed to delete booking.");
            }

            return NoContent();
        }

        [HttpGet("rooms")]
        public async Task<ActionResult<IEnumerable<AdminRoomDTO>>> GetRooms()
        {
            var rooms = await _context.Rooms
                .Include(r => r.RoomType) // Inkludera RoomType
                .ToListAsync();

            var roomDTOs = rooms.Select(room => new AdminRoomDTO
            {
                Id = room.Id,
                Number = room.Number,
                RoomType = room.RoomType != null ? new AdminRoomTypeDTO
                {
                    Id = room.RoomType.Id,
                    Name = room.RoomType.Name,
                    Description = room.RoomType.Description,
                    Price = room.RoomType.Price
                } : null,
                IsAvailable = room.IsAvailable
            });

            return Ok(roomDTOs);
        }

        [HttpGet("rooms/{id}")]
        public async Task<ActionResult<AdminRoomDTO>> GetRoomById(int id)
        {
            var room = await _context.Rooms
                .Include(r => r.RoomType)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (room == null)
            {
                return NotFound();
            }

            var roomDTO = new AdminRoomDTO
            {
                Id = room.Id,
                Number = room.Number,
                RoomType = room.RoomType != null ? new AdminRoomTypeDTO
                {
                    Id = room.RoomType.Id,
                    Name = room.RoomType.Name,
                    Description = room.RoomType.Description,
                    Price = room.RoomType.Price
                } : null,
                IsAvailable = room.IsAvailable
            };

            return Ok(roomDTO);
        }

        [HttpGet("available-rooms")]
        public async Task<ActionResult<IEnumerable<Room>>> GetAvailableRooms()
        {
            var rooms = await _context.Rooms
                .Include(r => r.RoomType)
                .ToListAsync();
            return Ok(rooms);
        }

        [HttpPut("rooms/{id}")]
        public async Task<IActionResult> UpdateRoom(int id, AdminRoomUpdateDTO roomUpdateDTO)
        {
            var room = await _context.Rooms
                .Include(r => r.RoomType)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (room == null)
            {
                return NotFound();
            }

            var roomType = await _context.RoomTypes.FirstOrDefaultAsync(rt => rt.Id == roomUpdateDTO.RoomTypeId);
            if (roomType == null)
            {
                return BadRequest("Invalid room type.");
            }

            room.Number = roomUpdateDTO.Number;
            room.RoomTypeId = roomType.Id;
            room.IsAvailable = roomUpdateDTO.IsAvailable;

            _context.Entry(room).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                return BadRequest("Failed to update room.");
            }

            var roomDTO = new AdminRoomDTO
            {
                Id = room.Id,
                Number = room.Number,
                RoomType = new AdminRoomTypeDTO
                {
                    Id = roomType.Id,
                    Name = roomType.Name,
                    Description = roomType.Description,
                    Price = roomType.Price
                },
                IsAvailable = room.IsAvailable
            };

            return Ok(roomDTO);
        }

        [HttpPost("rooms")]
        public async Task<ActionResult<AdminRoomDTO>> CreateRoom(AdminRoomCreateDTO roomCreateDTO)
        {
            var roomType = await _context.RoomTypes.FirstOrDefaultAsync(rt => rt.Id == roomCreateDTO.RoomTypeId);
            if (roomType == null)
            {
                return BadRequest("Invalid room type.");
            }

            var room = new Room
            {
                Number = roomCreateDTO.Number,
                RoomTypeId = roomType.Id,
                IsAvailable = roomCreateDTO.IsAvailable
            };

            _context.Rooms.Add(room);
            await _context.SaveChangesAsync();

            var roomDTO = new AdminRoomDTO
            {
                Id = room.Id,
                Number = room.Number,
                RoomType = new AdminRoomTypeDTO
                {
                    Id = roomType.Id,
                    Name = roomType.Name,
                    Description = roomType.Description,
                    Price = roomType.Price
                },
                IsAvailable = room.IsAvailable
            };

            return CreatedAtAction(nameof(GetRoomById), new { id = room.Id }, roomDTO);
        }

        [HttpDelete("rooms/{id}")]
        public async Task<IActionResult> DeleteRoom(int id)
        {
            var room = await _context.Rooms.FindAsync(id);

            if (room == null)
            {
                return NotFound();
            }

            _context.Rooms.Remove(room);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                return BadRequest("Failed to delete room.");
            }

            return NoContent();
        }

        [HttpGet("room-types")]
        public async Task<ActionResult<IEnumerable<AdminRoomTypeDTO>>> GetRoomTypes()
        {
            var roomTypes = await _context.RoomTypes
                .Select(rt => new AdminRoomTypeDTO
                {
                    Id = rt.Id,
                    Name = rt.Name,
                    Description = rt.Description,
                    Price = rt.Price
                })
                .ToListAsync();

            return Ok(roomTypes);
        }

        [HttpDelete("room-types/{id}")]
        public async Task<IActionResult> DeleteRoomType(int id)
        {
            var roomType = await _context.RoomTypes.FindAsync(id);

            if (roomType == null)
            {
                return NotFound();
            }

            _context.RoomTypes.Remove(roomType);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                return BadRequest("Failed to delete room type.");
            }

            return NoContent();
        }

        [HttpPost("room-types")]
        public async Task<ActionResult<AdminRoomTypeDTO>> CreateRoomType(AdminRoomTypeCreateDTO roomTypeCreateDTO)
        {
            var roomType = new RoomType
            {
                Name = roomTypeCreateDTO.Name,
                Description = roomTypeCreateDTO.Description,
                Price = roomTypeCreateDTO.Price
            };

            _context.RoomTypes.Add(roomType);
            await _context.SaveChangesAsync();

            var roomTypeDTO = new AdminRoomTypeDTO
            {
                Id = roomType.Id,
                Name = roomType.Name,
                Description = roomType.Description,
                Price = roomType.Price
            };

            return CreatedAtAction(nameof(GetRoomTypes), new { id = roomType.Id }, roomTypeDTO);
        }

        [HttpPut("room-types/{id}")]
        public async Task<IActionResult> UpdateRoomType(int id, AdminRoomTypeUpdateDTO roomTypeUpdateDTO)
        {
            if (id != roomTypeUpdateDTO.Id)
            {
                return BadRequest("Room type ID mismatch.");
            }

            var roomType = await _context.RoomTypes.FindAsync(id);

            if (roomType == null)
            {
                return NotFound();
            }

            roomType.Name = roomTypeUpdateDTO.Name;
            roomType.Description = roomTypeUpdateDTO.Description;
            roomType.Price = roomTypeUpdateDTO.Price;

            _context.Entry(roomType).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                return BadRequest("Failed to update room type.");
            }

            var roomTypeDTO = new AdminRoomTypeDTO
            {
                Id = roomType.Id,
                Name = roomType.Name,
                Description = roomType.Description,
                Price = roomType.Price
            };

            return Ok(roomTypeDTO);
        }
    }

    public class AdminDashboardDataDTO
    {
        public int TotalUsers { get; set; }
        public int TotalBookings { get; set; }
        public int TotalRooms { get; set; }
        public int TotalRoomTypes { get; set; }
    }

    public class AdminUserUpdateDTO
    {
        public string Id { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }

    public class AdminUserCreateDTO
    {
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    public class AdminBookingUpdateDTO
    {
        public int RoomId { get; set; }
        public string UserId { get; set; } = string.Empty;
        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }
    }

    public class AdminBookingCreateDTO
    {
        public string RoomType { get; set; } = string.Empty;
        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }
        public string UserId { get; set; } = string.Empty;
    }

    public class AdminRoomDTO
    {
        public int Id { get; set; }
        public string Number { get; set; } = string.Empty;
        public AdminRoomTypeDTO? RoomType { get; set; }
        public bool IsAvailable { get; set; }
    }

    public class AdminRoomUpdateDTO
    {
        public string Number { get; set; } = string.Empty;
        public int RoomTypeId { get; set; }
        public double Price { get; set; }
        public bool IsAvailable { get; set; }
    }

    public class AdminRoomCreateDTO
    {
        public string Number { get; set; } = string.Empty;
        public int RoomTypeId { get; set; }
        public double Price { get; set; }
        public bool IsAvailable { get; set; }
    }

    public class AdminRoomTypeDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public double Price { get; set; }
    }

    public class AdminRoomTypeCreateDTO
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public double Price { get; set; }
    }

    public class AdminRoomTypeUpdateDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public double Price { get; set; }
    }
}