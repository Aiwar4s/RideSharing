using FluentValidation;
using Microsoft.EntityFrameworkCore;
using O9d.AspNet.FluentValidation;
using webapi.Data;
using webapi.Data.Entities;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<TripDbContext>();
builder.Services.AddValidatorsFromAssemblyContaining<Program>();
var app = builder.Build();

var driversGroup = app.MapGroup("/api").WithValidationFilter();

driversGroup.MapGet("drivers", async (TripDbContext dbContext, CancellationToken cancellationToken) =>
{
    return (await dbContext.Drivers.ToListAsync(cancellationToken)).Select(driver => new DriverDto(driver.Id, driver.Name, driver.Email, driver.PhoneNumber, driver.IsVerified));
});
driversGroup.MapGet("drivers/{driverId}", async (int driverId, TripDbContext dbContext) =>
{
    var driver = await dbContext.Drivers.FirstOrDefaultAsync(d => d.Id == driverId);
    if (driver == null)
    {
        return Results.NotFound();
    }
    return Results.Ok(new DriverDto(driver.Id, driver.Name, driver.Email, driver.PhoneNumber, driver.IsVerified));
});
driversGroup.MapPost("drivers", async ([Validate] CreateDriverDto createDriverDto, TripDbContext dbContext) =>
{
    var driver = new Driver()
    {
        Name = createDriverDto.Name,
        Email = createDriverDto.Email,
        PhoneNumber = createDriverDto.PhoneNumber,
        IsVerified = false
    };
    dbContext.Drivers.Add(driver);
    await dbContext.SaveChangesAsync();
    return Results.Created($"/api/drivers/{driver.Id}", new DriverDto(driver.Id, driver.Name, driver.Email, driver.PhoneNumber, driver.IsVerified));
});

driversGroup.MapPut("drivers/{driverId}", async (int driverId, [Validate] UpdateDriverDto dto, TripDbContext dbContext) =>
{
    var driver = await dbContext.Drivers.FirstOrDefaultAsync(d => d.Id == driverId);
    if (driver == null)
    {
        return Results.NotFound();
    }
    driver.Email = dto.Email;
    driver.PhoneNumber = dto.PhoneNumber;
    driver.IsVerified = dto.IsVerified;
    dbContext.Update(driver);
    await dbContext.SaveChangesAsync();
    return Results.Ok(new DriverDto(driver.Id, driver.Name, driver.Email, driver.PhoneNumber, driver.IsVerified));
});
driversGroup.MapDelete("drivers/{driverId}", async (int driverId, TripDbContext dbContext) =>
{
    var driver = await dbContext.Drivers.FirstOrDefaultAsync(d => d.Id == driverId);
    if (driver == null)
    {
        return Results.NotFound();
    }
    var trips = await dbContext.Trips.Where(t => t.Driver.Id == driver.Id).ToListAsync();
    foreach(Trip trip in trips)
    {
        var reviews=await dbContext.Reviews.Where(r=>r.Trip.Id== trip.Id).ToListAsync();
        foreach(Review review in reviews)
        {
            dbContext.Remove(review);
        }
        dbContext.Remove(trip);
    }
    dbContext.Remove(driver);
    await dbContext.SaveChangesAsync();
    return Results.NoContent();
});

var tripsGroup = app.MapGroup("/api/drivers/{driverId}").WithValidationFilter();

tripsGroup.MapGet("trips", async (int driverId, TripDbContext dbContext, CancellationToken cancellationToken) =>
{
    return (await dbContext.Trips.Where(t => t.Driver.Id == driverId).ToListAsync(cancellationToken))
    .Select(trip => new TripDto(trip.Id, trip.Departure, trip.Destination, trip.Time, trip.Seats, trip.Description));
});
tripsGroup.MapGet("trips/{tripId}", async ([AsParameters] GetTripParameters parameters) =>
{
    var trip = await parameters.dbContext.Trips.FirstOrDefaultAsync(t => t.Id == parameters.tripId && t.Driver.Id == parameters.driverId);
    if(trip == null)
    {
        return Results.NotFound();
    }
    return Results.Ok(new TripDto(trip.Id, trip.Departure, trip.Destination, trip.Time, trip.Seats, trip.Description));
});
tripsGroup.MapPost("trips", async ([Validate] CreateTripDto createTripDto, int driverId, TripDbContext dbContext) =>
{
    var driver = await dbContext.Drivers.FirstOrDefaultAsync(d => d.Id == driverId);
    if (driver == null)
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
        Driver = driver
    };
    dbContext.Trips.Add(trip);
    await dbContext.SaveChangesAsync();
    return Results.Created($"/api/drivers/{driver.Id}/trips/{trip.Id}", new TripDto(trip.Id, trip.Departure, trip.Destination, trip.Time, trip.Seats, trip.Description));
});
tripsGroup.MapPut("trips/{tripId}", async ([AsParameters] GetTripParameters parameters, [Validate] UpdateTripDto dto) =>
{
    var trip = await parameters.dbContext.Trips.FirstOrDefaultAsync(t=>t.Id==parameters.tripId && t.Driver.Id==parameters.driverId);
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
    var trip = await parameters.dbContext.Trips.FirstOrDefaultAsync(t => t.Driver.Id == parameters.driverId && t.Id == parameters.tripId);
    if (trip == null)
    {
        return Results.NotFound();
    }
    var reviews = await parameters.dbContext.Reviews.Where(r => r.Trip.Id == trip.Id).ToListAsync();
    foreach (Review review in reviews)
    {
        parameters.dbContext.Remove(review);
    }
    parameters.dbContext.Remove(trip);
    await parameters.dbContext.SaveChangesAsync();
    return Results.NoContent();
});

var reviewsGroup = app.MapGroup("/api/drivers/{driverId}/trips/{tripId}").WithValidationFilter();

reviewsGroup.MapGet("reviews", async (int driverId, int tripId, TripDbContext dbContext, CancellationToken cancellationToken) =>
{
    var reviews = (await dbContext.Reviews.Where(r => r.Trip.Id == tripId && r.Trip.Driver.Id == driverId).ToListAsync(cancellationToken));
    return reviews.Select(review => new ReviewDto(review.Id, review.Rating, review.Description, review.ReviewerId));
});
reviewsGroup.MapGet("reviews/{reviewId}", async ([AsParameters] GetReviewParameters parameters) =>
{
    var review = await parameters.dbContext.Reviews
    .FirstOrDefaultAsync(r => r.Id == parameters.reviewId && r.Trip.Id == parameters.tripId && r.Trip.Driver.Id == parameters.driverId);
    if (review == null)
    {
        return Results.NotFound();
    }
    return Results.Ok(new ReviewDto(review.Id, review.Rating, review.Description, review.ReviewerId));
});
reviewsGroup.MapPost("reviews", async ([Validate] CreateReviewDto createReviewDto, int driverId, int tripId, TripDbContext dbContext) =>
{
    var trip = await dbContext.Trips.FirstOrDefaultAsync(t => t.Id == tripId && t.Driver.Id == driverId);
    var reviewer = await dbContext.Users.FirstOrDefaultAsync(u => u.Id == createReviewDto.ReviewerId);
    if (trip == null || reviewer == null)
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
    return Results.Created($"/api/drivers/{driverId}/trips/{tripId}/reviews/{review.Id}", new ReviewDto(review.Id, review.Rating, review.Description, review.ReviewerId));
});
reviewsGroup.MapPut("reviews/{reviewId}", async ([AsParameters] GetReviewParameters parameters, [Validate] UpdateReviewDto dto) =>
{
    var review = await parameters.dbContext.Reviews
    .FirstOrDefaultAsync(r => r.Id == parameters.reviewId && r.Trip.Id == parameters.tripId && r.Trip.Driver.Id == parameters.driverId);
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
    .FirstOrDefaultAsync(r => r.Id==parameters.reviewId && r.Trip.Id == parameters.tripId && r.Trip.Driver.Id == parameters.driverId);
    if (review == null)
    {
        return Results.NotFound();
    }
    parameters.dbContext.Remove(review);
    await parameters.dbContext.SaveChangesAsync();
    return Results.NoContent();
});

app.Run();
