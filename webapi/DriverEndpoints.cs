using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.JsonWebTokens;
using O9d.AspNet.FluentValidation;
using System.Net.Http;
using System.Security.Claims;
using System.Text.Json;
using webapi.Auth.Entities;
using webapi.Data;
using webapi.Data.Entities;
using webapi.Helpers;

namespace webapi
{
    public static class DriverEndpoints
    {
        public static void AddDriverApi(this WebApplication app)
        {
            var driversGroup = app.MapGroup("/api").WithValidationFilter();

            driversGroup.MapGet("drivers", async ([AsParameters] SearchParameters searchParams, TripDbContext dbContext, LinkGenerator linkGenerator, HttpContext httpContext) =>
            {
                var queryable = dbContext.Drivers.AsQueryable().OrderBy(o => o.Id);
                var pagedList = await PagedList<Driver>.CreateAsync(queryable, searchParams.PageNumber!.Value, searchParams.PageSize!.Value);

                var previousPageLink =
                    pagedList.HasPrevious
                        ? linkGenerator.GetUriByName(httpContext, "GetDrivers", new { pageNumber = searchParams.PageNumber - 1, pageSize = searchParams.PageSize })
                        : null;
                var nextPageLink =
                    pagedList.HasNext
                        ? linkGenerator.GetUriByName(httpContext, "GetDrivers", new { pageNumber = searchParams.PageNumber + 1, pageSize = searchParams.PageSize })
                        : null;

                var paginationMetadata=new PaginationMetadata(pagedList.TotalCount, pagedList.PageSize, pagedList.CurrentPage, pagedList.TotalPages,
                    previousPageLink, nextPageLink);

                httpContext.Response.Headers.Add("Pagination", JsonSerializer.Serialize(paginationMetadata));
                return pagedList.Select(driver => new DriverDto(driver.Id, driver.Name, driver.Email, driver.PhoneNumber));
            }).WithName("GetDrivers");
            driversGroup.MapGet("drivers/{driverId}", async (int driverId, TripDbContext dbContext) =>
            {
                var driver = await dbContext.Drivers.FirstOrDefaultAsync(d => d.Id == driverId);
                if (driver == null)
                {
                    return Results.NotFound();
                }
                return Results.Ok(new DriverDto(driver.Id, driver.Name, driver.Email, driver.PhoneNumber));
            }).WithName("GetDriver");
            driversGroup.MapPost("drivers", [Authorize(Roles = UserRoles.Admin)] async ([Validate] CreateDriverDto createDriverDto, HttpContext httpContext, LinkGenerator linkGenerator, TripDbContext dbContext, UserManager<User> userManager) =>
            {
                var user = await userManager.FindByIdAsync(createDriverDto.UserId);
                if (user == null)
                {
                    return Results.NotFound();
                }
                var driver = new Driver()
                {
                    Name = createDriverDto.Name,
                    Email = createDriverDto.Email,
                    PhoneNumber = createDriverDto.PhoneNumber,
                    UserId=createDriverDto.UserId,
                    User=user
                };
                dbContext.Drivers.Add(driver);
                await dbContext.SaveChangesAsync();

                var links = CreateLinks(driver.Id, httpContext, linkGenerator);
                var driverDto = new DriverDto(driver.Id, driver.Name, driver.Email, driver.PhoneNumber);
                var resource = new ResourceDto<DriverDto>(driverDto, links.ToArray());

                await userManager.AddToRoleAsync(user, UserRoles.Driver);

                return Results.Created($"/api/drivers/{driver.Id}", resource);
            }).WithName("CreateDriver");

            driversGroup.MapPut("drivers/{driverId}", [Authorize(Roles = UserRoles.Driver)] async (int driverId, [Validate] UpdateDriverDto dto, TripDbContext dbContext, HttpContext httpContext) =>
            {
                var driver = await dbContext.Drivers.FirstOrDefaultAsync(d => d.Id == driverId);
                if (driver == null)
                {
                    return Results.NotFound();
                }
                if (!httpContext.User.IsInRole(UserRoles.Admin) && httpContext.User.FindFirstValue(JwtRegisteredClaimNames.Sub) != driver.UserId)
                {
                    return Results.Forbid();
                }
                driver.Email = dto.Email;
                driver.PhoneNumber = dto.PhoneNumber;
                dbContext.Update(driver);
                await dbContext.SaveChangesAsync();
                return Results.Ok(new DriverDto(driver.Id, driver.Name, driver.Email, driver.PhoneNumber));
            }).WithName("EditDriver");
            driversGroup.MapDelete("drivers/{driverId}", [Authorize(Roles = UserRoles.Driver)] async (int driverId, TripDbContext dbContext, HttpContext httpContext) =>
            {
                var driver = await dbContext.Drivers.FirstOrDefaultAsync(d => d.Id == driverId);
                if (driver == null)
                {
                    return Results.NotFound();
                }
                if (!httpContext.User.IsInRole(UserRoles.Admin) && httpContext.User.FindFirstValue(JwtRegisteredClaimNames.Sub) != driver.UserId)
                {
                    return Results.Forbid();
                }
                var trips = await dbContext.Trips.Where(t => t.Driver.Id == driver.Id).ToListAsync();
                foreach (Trip trip in trips)
                {
                    var reviews = await dbContext.Reviews.Where(r => r.Trip.Id == trip.Id).ToListAsync();
                    foreach (Review review in reviews)
                    {
                        dbContext.Remove(review);
                    }
                    dbContext.Remove(trip);
                }
                dbContext.Remove(driver);
                await dbContext.SaveChangesAsync();
                return Results.NoContent();
            }).WithName("RemoveDriver");
        }
        static IEnumerable<LinkDto> CreateLinks(int driverId, HttpContext httpContext, LinkGenerator linkGenerator)
        {
            yield return new LinkDto(linkGenerator.GetUriByName(httpContext, "GetDriver", new { driverId }), "self", "GET");
            yield return new LinkDto(linkGenerator.GetUriByName(httpContext, "EditDriver", new { driverId }), "edit", "PUT");
            yield return new LinkDto(linkGenerator.GetUriByName(httpContext, "RemoveDriver", new { driverId }), "delete", "GET");
        }
    }
}
