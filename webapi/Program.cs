using FluentValidation;
using Microsoft.EntityFrameworkCore;
using O9d.AspNet.FluentValidation;
using webapi.Data;
using webapi.Data.Entities;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<TripDbContext>();
builder.Services.AddValidatorsFromAssemblyContaining<Program>();
var app = builder.Build();

var usersGroup = app.MapGroup("/api").WithValidationFilter();

usersGroup.MapGet("users", async (TripDbContext dbContext, CancellationToken cancellationToken) =>
{
    return (await dbContext.Users.ToListAsync(cancellationToken)).Select(user => new UserDto(user.Id, user.Name, user.Email, user.PhoneNumber));
});
usersGroup.MapGet("users/{userId}", async (int userId, TripDbContext dbContext) =>
{
    var user = await dbContext.Users.FirstOrDefaultAsync(u => u.Id == userId);
    if (user == null)
    {
        return Results.NotFound();
    }
    return Results.Ok(new UserDto(user.Id, user.Name, user.Email, user.PhoneNumber));
});
usersGroup.MapPost("users", async ([Validate] CreateUserDto createUserDto, TripDbContext dbContext) =>
{
    var user = new User()
    {
        Name = createUserDto.Name,
        Email = createUserDto.Email,
        Password = createUserDto.Password,
        PhoneNumber = createUserDto.PhoneNumber
    };
    dbContext.Users.Add(user);
    await dbContext.SaveChangesAsync();
    return Results.Created($"/api/users/{user.Id}", new UserDto(user.Id, user.Name, user.Email, user.PhoneNumber));
});
usersGroup.MapPut("users/{userId}", async (int userId, [Validate] UpdateUserDto dto, TripDbContext dbContext) =>
{
    var user = await dbContext.Users.FirstOrDefaultAsync(u => u.Id == userId);
    if (user == null)
    {
        return Results.NotFound();
    }
    user.Email = dto.Email;
    user.Password = dto.Password;
    user.PhoneNumber = dto.PhoneNumber;
    dbContext.Update(user);
    await dbContext.SaveChangesAsync();
    return Results.Ok(new UserDto(user.Id, user.Name, user.Email, user.PhoneNumber));
});
usersGroup.MapDelete("users/{userId}", async (int userId, TripDbContext dbContext) =>
{
    var user = await dbContext.Users.FirstOrDefaultAsync(u => u.Id == userId);
    if (user == null)
    {
        return Results.NotFound();
    }
    dbContext.Remove(user);
    await dbContext.SaveChangesAsync();
    return Results.NoContent();
});

var tripsGroup = app.MapGroup("/api/users/{userId}").WithValidationFilter();

tripsGroup.MapGet("trips", async (int userId, TripDbContext dbContext, CancellationToken cancellationToken) =>
{
    return (await dbContext.Trips.Where(t => t.Driver.Id == userId).ToListAsync(cancellationToken))
    .Select(trip => new TripDto(trip.Id, trip.Departure, trip.Destination, trip.Time, trip.Seats, trip.Description));
});
tripsGroup.MapGet("trips/{tripId}", async ([AsParameters] GetTripParameters parameters) =>
{
    var trip = await parameters.dbContext.Trips.FirstOrDefaultAsync(t => t.Id == parameters.tripId && t.Driver.Id == parameters.userId);
    if(trip == null)
    {
        return Results.NotFound();
    }
    return Results.Ok(new TripDto(trip.Id, trip.Departure, trip.Destination, trip.Time, trip.Seats, trip.Description));
});
tripsGroup.MapPost("trips", async ([Validate] CreateTripDto createTripDto, int userId, TripDbContext dbContext) =>
{
    var user = await dbContext.Users.FirstOrDefaultAsync(u => u.Id == userId);
    if (user == null)
    {
        return Results.NotFound();
    }
    var trip = new Trip()
    {
        Departure = createTripDto.Departure,
        Destination = createTripDto.Destination,
        Time = createTripDto.Time,
        Seats = createTripDto.Seats,
        Description = createTripDto.Description,
        Driver = user
    };
    dbContext.Trips.Add(trip);
    await dbContext.SaveChangesAsync();
    return Results.Created($"/api/users/{user.Id}/trips/{trip.Id}", new TripDto(trip.Id, trip.Departure, trip.Destination, trip.Time, trip.Seats, trip.Description));
});
tripsGroup.MapPut("trips/{tripId}", async ([AsParameters] GetTripParameters parameters, [Validate] UpdateTripDto dto) =>
{
    var trip = await parameters.dbContext.Trips.FirstOrDefaultAsync(t=>t.Id==parameters.tripId && t.Driver.Id==parameters.userId);
    if (trip==null)
    {
        return Results.NotFound();
    }
    trip.Time= dto.Time;
    trip.Seats= dto.Seats;
    trip.Description= dto.Description;
    parameters.dbContext.Update(trip);
    await parameters.dbContext.SaveChangesAsync();
    return Results.Ok(new TripDto(trip.Id, trip.Departure, trip.Destination, trip.Time, trip.Seats, trip.Description));
});
tripsGroup.MapDelete("trips/{tripId}", async ([AsParameters] GetTripParameters parameters) =>
{
    var trip = await parameters.dbContext.Trips.FirstOrDefaultAsync(t => t.Driver.Id == parameters.userId && t.Id == parameters.tripId);
    if (trip == null)
    {
        return Results.NotFound();
    }
    parameters.dbContext.Remove(trip);
    await parameters.dbContext.SaveChangesAsync();
    return Results.NoContent();
});

var reviewsGroup = app.MapGroup("/api/users/{userId}/trips/{tripId}").WithValidationFilter();

reviewsGroup.MapGet("reviews", async (int userId, int tripId, TripDbContext dbContext, CancellationToken cancellationToken) =>
{
    var reviews = (await dbContext.Reviews.Where(r => r.Trip.Id == tripId && r.Trip.Driver.Id == userId).ToListAsync(cancellationToken));
    return reviews.Select(review => new ReviewDto(review.Id, review.Rating, review.Description, review.ReviewerId));
});
reviewsGroup.MapGet("reviews/{reviewId}", async ([AsParameters] GetReviewParameters parameters) =>
{
    var review = await parameters.dbContext.Reviews
    .FirstOrDefaultAsync(r => r.Id == parameters.reviewId && r.Trip.Id == parameters.tripId && r.Trip.Driver.Id==parameters.userId);
    if (review == null)
    {
        return Results.NotFound();
    }
    return Results.Ok(new ReviewDto(review.Id, review.Rating, review.Description, review.ReviewerId));
});
reviewsGroup.MapPost("reviews", async ([Validate] CreateReviewDto createReviewDto, int userId, int tripId, TripDbContext dbContext) =>
{
    var trip = await dbContext.Trips.FirstOrDefaultAsync(t => t.Id == tripId && t.Driver.Id==userId);
    var reviewer = await dbContext.Users.FirstOrDefaultAsync(u => u.Id == createReviewDto.ReviewerId);
    if (trip == null || reviewer==null)
    {
        return Results.NotFound();
    }
    var review = new Review()
    {
        Rating = createReviewDto.Rating,
        Description = createReviewDto.Description,
        ReviewerId = reviewer.Id,
        Trip = trip
    };
    dbContext.Reviews.Add(review);
    await dbContext.SaveChangesAsync();
    return Results.Created($"/api/users/{userId}/trips/{tripId}/reviews/{review.Id}", new ReviewDto(review.Id, review.Rating, review.Description, review.ReviewerId));
});
reviewsGroup.MapPut("reviews/{reviewId}", async ([AsParameters] GetReviewParameters parameters, [Validate] UpdateReviewDto dto) =>
{
    var review = await parameters.dbContext.Reviews
    .FirstOrDefaultAsync(r => r.Id == parameters.reviewId && r.Trip.Id == parameters.tripId && r.Trip.Driver.Id==parameters.userId);
    if (review == null)
    {
        return Results.NotFound();
    }
    review.Rating = dto.Rating;
    review.Description = dto.Description;
    parameters.dbContext.Update(review);
    await parameters.dbContext.SaveChangesAsync();
    return Results.Ok(new ReviewDto(review.Id, review.Rating, review.Description, review.ReviewerId));
});
reviewsGroup.MapDelete("reviews/{reviewId}", async ([AsParameters] GetReviewParameters parameters) =>
{
    var review = await parameters.dbContext.Reviews
    .FirstOrDefaultAsync(r => r.Id==parameters.reviewId && r.Trip.Id == parameters.tripId && r.Trip.Driver.Id == parameters.userId);
    if (review == null)
    {
        return Results.NotFound();
    }
    parameters.dbContext.Remove(review);
    await parameters.dbContext.SaveChangesAsync();
    return Results.NoContent();
});

app.Run();
