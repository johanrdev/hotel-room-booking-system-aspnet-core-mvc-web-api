using HotelBookingSystem.API.Data;
using HotelBookingSystem.API.Models;

public static class DbInitializer
{
    public static void Initialize(AppDbContext context)
    {
        // Kontrollera om databasen redan är seedad
        if (context.Rooms.Any())
        {
            return;   // DB har redan seedats
        }

        var rooms = new Room[]
        {
            new Room { Number = "101", Type = "Single", Price = 100.0, IsAvailable = true },
            new Room { Number = "102", Type = "Double", Price = 150.0, IsAvailable = true },
            // new Room { Number = "103", Type = "Single", Price = 100.0, IsAvailable = true },
            // new Room { Number = "104", Type = "Double", Price = 150.0, IsAvailable = true },
            // new Room { Number = "105", Type = "Single", Price = 100.0, IsAvailable = true },
            // new Room { Number = "106", Type = "Double", Price = 150.0, IsAvailable = true },
            // new Room { Number = "107", Type = "Single", Price = 100.0, IsAvailable = true },
            // new Room { Number = "108", Type = "Double", Price = 150.0, IsAvailable = true },
            // new Room { Number = "109", Type = "Single", Price = 100.0, IsAvailable = true },
            // new Room { Number = "110", Type = "Double", Price = 150.0, IsAvailable = true },
            // new Room { Number = "111", Type = "Single", Price = 100.0, IsAvailable = true },
            // new Room { Number = "112", Type = "Double", Price = 150.0, IsAvailable = true },
            // new Room { Number = "113", Type = "Single", Price = 100.0, IsAvailable = true },
            // new Room { Number = "114", Type = "Double", Price = 150.0, IsAvailable = true },
            // new Room { Number = "115", Type = "Single", Price = 100.0, IsAvailable = true },
            // new Room { Number = "116", Type = "Double", Price = 150.0, IsAvailable = true },
            // new Room { Number = "117", Type = "Single", Price = 100.0, IsAvailable = true },
            // new Room { Number = "118", Type = "Double", Price = 150.0, IsAvailable = true },
            // new Room { Number = "119", Type = "Single", Price = 100.0, IsAvailable = true },
            // new Room { Number = "120", Type = "Double", Price = 150.0, IsAvailable = true }
        };

        foreach (var room in rooms)
        {
            context.Rooms.Add(room);
        }
        context.SaveChanges();
    }
}
