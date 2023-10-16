using webapi.Data;
using webapi.Data.Entities;

namespace webapi.Data.Entities
{
    public class Review
    {
        public int Id { get; set; }
        public required int Rating { get; set; }
        public string? Description { get; set; }
        public required int ReviewerId { get; set; }
        public required Trip Trip { get; set; }
    }
}

public record GetReviewParameters(int driverId, int tripId, int reviewId, TripDbContext dbContext);
public record ReviewDto(int Id, int Rating, string? Description, int ReviewerId);
public record CreateReviewDto(int Rating, string? Description, int ReviewerId);
public record UpdateReviewDto(int Rating, string? Description);