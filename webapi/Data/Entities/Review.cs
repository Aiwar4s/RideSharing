using System.ComponentModel.DataAnnotations;
using webapi.Auth.Entities;
using webapi.Data;
using webapi.Data.Entities;

namespace webapi.Data.Entities
{
    public class Review
    {
        public int Id { get; set; }
        public required int Rating { get; set; }
        public string? Description { get; set; }
        public required Trip Trip { get; set; }
        [Required]
        public required string UserId { get; set; }
        public User User { get; set; }
    }
}

public record GetReviewParameters(int driverId, int tripId, int reviewId, TripDbContext dbContext);
public record ReviewDtoTemp(int Id, int Rating, string? Description, string ReviewerId);
public record ReviewDto(int Id, int Rating, string? Description, string ReviewerId, string Reviewer);
public record CreateReviewDto(int Rating, string? Description);
public record UpdateReviewDto(int Rating, string? Description);