using Microsoft.EntityFrameworkCore;
using O9d.AspNet.FluentValidation;
using webapi.Data.Entities;
using webapi.Data;
using webapi.Helpers;
using System.Text.Json;
using System.Security.Claims;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.AspNetCore.Authorization;
using webapi.Auth.Entities;
using Microsoft.AspNetCore.Http;

namespace webapi
{
    public static class TripEndpoints
    {
        public static void AddTripApi(this WebApplication app)
        {
            var tripsGroup = app.MapGroup("/api/drivers/{driverId}").WithValidationFilter();

            tripsGroup.MapGet("trips", async ([AsParameters] SearchParameters searchParams, int driverId, TripDbContext dbContext, LinkGenerator linkGenerator, HttpContext httpContext) =>
            {
                var queryable = dbContext.Trips.Where(t=>t.Driver.Id==driverId).AsQueryable().OrderBy(o => o.Id);
                var pagedList = await PagedList<Trip>.CreateAsync(queryable, searchParams.PageNumber!.Value, searchParams.PageSize!.Value);

                var previousPageLink =
                    pagedList.HasPrevious
                        ? linkGenerator.GetUriByName(httpContext, "GetTrips", new { pageNumber = searchParams.PageNumber - 1, pageSize = searchParams.PageSize })
                        : null;
                var nextPageLink =
                    pagedList.HasNext
                        ? linkGenerator.GetUriByName(httpContext, "GetTrips", new { pageNumber = searchParams.PageNumber + 1, pageSize = searchParams.PageSize })
                        : null;

                var paginationMetadata = new PaginationMetadata(pagedList.TotalCount, pagedList.PageSize, pagedList.CurrentPage, pagedList.TotalPages,
                    previousPageLink, nextPageLink);

                httpContext.Response.Headers.Add("Pagination", JsonSerializer.Serialize(paginationMetadata));
                return pagedList.Select(trip => new TripDto(trip.Id, trip.Departure, trip.Destination, trip.Time, trip.Seats, trip.Description, trip.UserId));
            }).WithName("GetTrips");

            tripsGroup.MapGet("trips/{tripId}", async ([AsParameters] GetTripParameters parameters) =>
            {
                var trip = await parameters.dbContext.Trips.FirstOrDefaultAsync(t => t.Id == parameters.tripId && t.Driver.Id == parameters.driverId);
                if (trip == null)
                {
                    return Results.NotFound();
                }
                return Results.Ok(new TripDto(trip.Id, trip.Departure, trip.Destination, trip.Time, trip.Seats, trip.Description, trip.UserId));
            }).WithName("GetTrip");

            tripsGroup.MapPost("trips", [Authorize(Roles =UserRoles.Driver)] async ([Validate] CreateTripDto createTripDto, int driverId, HttpContext httpContext, LinkGenerator linkGenerator, TripDbContext dbContext) =>
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
                    Driver = driver,
                    UserId = driver.UserId
                };
                dbContext.Trips.Add(trip);
                await dbContext.SaveChangesAsync();

                var links = CreateLinks(trip.Id, httpContext, linkGenerator);
                var tripDto = new TripDto(trip.Id, trip.Departure, trip.Destination, trip.Time, trip.Seats, trip.Description, trip.UserId);
                var resource = new ResourceDto<TripDto>(tripDto, links.ToArray());

                return Results.Created($"/api/drivers/{driver.Id}/trips/{trip.Id}", resource);
            }).WithName("CreateTrip");

            tripsGroup.MapPut("trips/{tripId}", [Authorize(Roles = UserRoles.Driver)] async ([AsParameters] GetTripParameters parameters, HttpContext httpContext, [Validate] UpdateTripDto dto) =>
            {
                var trip = await parameters.dbContext.Trips.FirstOrDefaultAsync(t => t.Id == parameters.tripId && t.Driver.Id == parameters.driverId);
                if (trip == null)
                {
                    return Results.NotFound();
                }

                if(!httpContext.User.IsInRole(UserRoles.Admin) && httpContext.User.FindFirstValue(JwtRegisteredClaimNames.Sub) != trip.UserId)
                {
                    return Results.Forbid();
                }

                trip.Time = dto.Time;
                trip.Seats = dto.Seats;
                trip.Description = dto.Description;
                parameters.dbContext.Update(trip);
                await parameters.dbContext.SaveChangesAsync();
                return Results.Ok(new TripDto(trip.Id, trip.Departure, trip.Destination, trip.Time, trip.Seats, trip.Description, trip.UserId));
            }).WithName("EditTrip");

            tripsGroup.MapDelete("trips/{tripId}", [Authorize(Roles = UserRoles.Driver)] async ([AsParameters] GetTripParameters parameters, HttpContext httpContext) =>
            {
                var trip = await parameters.dbContext.Trips.FirstOrDefaultAsync(t => t.Driver.Id == parameters.driverId && t.Id == parameters.tripId);
                if (trip == null)
                {
                    return Results.NotFound();
                }
                if (!httpContext.User.IsInRole(UserRoles.Admin) && httpContext.User.FindFirstValue(JwtRegisteredClaimNames.Sub) != trip.UserId)
                {
                    return Results.Forbid();
                }
                var reviews = await parameters.dbContext.Reviews.Where(r => r.Trip.Id == trip.Id).ToListAsync();
                foreach (Review review in reviews)
                {
                    parameters.dbContext.Remove(review);
                }
                parameters.dbContext.Remove(trip);
                await parameters.dbContext.SaveChangesAsync();
                return Results.NoContent();
            }).WithName("RemoveTrip");
        }
        static IEnumerable<LinkDto> CreateLinks(int tripId, HttpContext httpContext, LinkGenerator linkGenerator)
        {
            yield return new LinkDto(linkGenerator.GetUriByName(httpContext, "GetTrip", new { tripId }), "self", "GET");
            yield return new LinkDto(linkGenerator.GetUriByName(httpContext, "EditTrip", new { tripId }), "edit", "PUT");
            yield return new LinkDto(linkGenerator.GetUriByName(httpContext, "RemoveTrip", new { tripId }), "delete", "GET");
        }
    }
}
