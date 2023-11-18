using System.ComponentModel.DataAnnotations;
using webapi.Auth.Entities;
using webapi.Data;

namespace webapi.Data.Entities
{
    public class Trip
    {
        public int Id { get; set; }
        public required string Departure { get; set; }
        public required string Destination { get; set; }
        public required DateTime Time { get; set; }
        public required int Seats {  get; set; }
        public string? Description { get; set; }
        public required Driver Driver { get; set; }
        [Required]
        public required string UserId { get; set; }
        public User User { get; set; }
    }
}

public record GetTripParameters(int driverId, int tripId, TripDbContext dbContext);
public record TripDto(int Id, string Departure, string Destination, DateTime Time, int Seats, string? Description);
public record CreateTripDto(string Departure, string Destination, DateTime Time, int Seats, string? Description);
public record UpdateTripDto(DateTime Time, int Seats, string? Description);