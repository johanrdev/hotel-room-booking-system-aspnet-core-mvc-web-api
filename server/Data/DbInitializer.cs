using HotelBookingSystem.API.Data;
using HotelBookingSystem.API.Models;

public static class DbInitializer
{
    public static void Initialize(AppDbContext context)
    {
        if (context.Rooms.Any())
        {
            return;
        }

        var roomTypes = new RoomType[]
        {
            new RoomType { Name = "Single", Description = "A single room with one bed.", Price = 100.0, ImageUrl = "single.jpg" },
            new RoomType { Name = "Double", Description = "A double room with two beds.", Price = 150.0, ImageUrl = "double.jpg" },
            new RoomType { Name = "Suite", Description = "A suite with a living area.", Price = 250.0, ImageUrl = "suite.jpg" }
        };

        foreach (var roomType in roomTypes)
        {
            context.RoomTypes.Add(roomType);
        }
        context.SaveChanges();

        var rooms = new Room[]
        {
            new Room { Number = "101", RoomTypeId = roomTypes.First(rt => rt.Name == "Single").Id, IsAvailable = true },
            new Room { Number = "102", RoomTypeId = roomTypes.First(rt => rt.Name == "Double").Id, IsAvailable = true },
            new Room { Number = "201", RoomTypeId = roomTypes.First(rt => rt.Name == "Suite").Id, IsAvailable = true }
        };

        foreach (var room in rooms)
        {
            context.Rooms.Add(room);
        }
        context.SaveChanges();
    }
}
