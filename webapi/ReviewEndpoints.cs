using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
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
    public static class ReviewEndpoints
    {
        public static void AddReviewApi(this WebApplication app)
        {
            var reviewsGroup = app.MapGroup("/api/drivers/{driverId}/trips/{tripId}").WithValidationFilter();

            reviewsGroup.MapGet("reviews", async ([AsParameters] SearchParameters searchParams, int driverId, int tripId, TripDbContext dbContext, LinkGenerator linkGenerator, HttpContext httpContext) =>
            {
                var queryable = dbContext.Reviews.Where(r => r.Trip.Id == tripId && r.Trip.Driver.Id == driverId).AsQueryable().OrderBy(o => o.Id);
                var pagedList = await PagedList<Review>.CreateAsync(queryable, searchParams.PageNumber!.Value, searchParams.PageSize!.Value);
                var previousPageLink =
                    pagedList.HasPrevious
                        ? linkGenerator.GetUriByName(httpContext, "GetReviews", new { pageNumber = searchParams.PageNumber - 1, pageSize = searchParams.PageSize })
                        : null;
                var nextPageLink =
                    pagedList.HasNext
                        ? linkGenerator.GetUriByName(httpContext, "GetReviews", new { pageNumber = searchParams.PageNumber + 1, pageSize = searchParams.PageSize })
                        : null;

                var paginationMetadata = new PaginationMetadata(pagedList.TotalCount, pagedList.PageSize, pagedList.CurrentPage, pagedList.TotalPages,
                    previousPageLink, nextPageLink);

                httpContext.Response.Headers.Add("Pagination", JsonSerializer.Serialize(paginationMetadata));
                return pagedList.Select(review => new ReviewDto(review.Id, review.Rating, review.Description, review.UserId));
            }).WithName("GetReviews");

            reviewsGroup.MapGet("reviews/{reviewId}", async ([AsParameters] GetReviewParameters parameters) =>
            {
                var review = await parameters.dbContext.Reviews
                .FirstOrDefaultAsync(r => r.Id == parameters.reviewId && r.Trip.Id == parameters.tripId && r.Trip.Driver.Id == parameters.driverId);
                if (review == null)
                {
                    return Results.NotFound();
                }
                return Results.Ok(new ReviewDto(review.Id, review.Rating, review.Description, review.UserId));
            }).WithName("GetReview");

            reviewsGroup.MapPost("reviews", [Authorize(Roles = UserRoles.BasicUser)] async ([Validate] CreateReviewDto createReviewDto, int driverId, int tripId, HttpContext httpContext, LinkGenerator linkGenerator, TripDbContext dbContext) =>
            {
                var trip = await dbContext.Trips.FirstOrDefaultAsync(t => t.Id == tripId && t.Driver.Id == driverId);
                if (trip == null)
                {
                    return Results.NotFound();
                }
                var review = new Review()
                {
                    Rating = createReviewDto.Rating,
                    Description = createReviewDto.Description,
                    Trip = trip,
                    UserId = httpContext.User.FindFirstValue(JwtRegisteredClaimNames.Sub)
                };
                dbContext.Reviews.Add(review);
                await dbContext.SaveChangesAsync();

                var links = CreateLinks(review.Id, httpContext, linkGenerator);
                var reviewDto = new ReviewDto(review.Id, review.Rating, review.Description, review.UserId);
                var resource = new ResourceDto<ReviewDto>(reviewDto, links.ToArray());

                return Results.Created($"/api/drivers/{driverId}/trips/{tripId}/reviews/{review.Id}", resource);
            }).WithName("CreateReview");

            reviewsGroup.MapPut("reviews/{reviewId}", [Authorize(Roles = UserRoles.BasicUser)] async ([AsParameters] GetReviewParameters parameters, [Validate] UpdateReviewDto dto, HttpContext httpContext) =>
            {
                var review = await parameters.dbContext.Reviews
                .FirstOrDefaultAsync(r => r.Id == parameters.reviewId && r.Trip.Id == parameters.tripId && r.Trip.Driver.Id == parameters.driverId);
                if (review == null)
                {
                    return Results.NotFound();
                }
                if (!httpContext.User.IsInRole(UserRoles.Admin) && httpContext.User.FindFirstValue(JwtRegisteredClaimNames.Sub) != review.UserId)
                {
                    return Results.Forbid();
                }
                review.Rating = dto.Rating;
                review.Description = dto.Description;
                parameters.dbContext.Update(review);
                await parameters.dbContext.SaveChangesAsync();
                return Results.Ok(new ReviewDto(review.Id, review.Rating, review.Description, review.UserId));
            }).WithName("EditReview");

            reviewsGroup.MapDelete("reviews/{reviewId}", [Authorize(Roles = UserRoles.BasicUser)] async ([AsParameters] GetReviewParameters parameters, HttpContext httpContext) =>
            {
                var review = await parameters.dbContext.Reviews
                .FirstOrDefaultAsync(r => r.Id == parameters.reviewId && r.Trip.Id == parameters.tripId && r.Trip.Driver.Id == parameters.driverId);
                if (review == null)
                {
                    return Results.NotFound();
                }
                if (!httpContext.User.IsInRole(UserRoles.Admin) && httpContext.User.FindFirstValue(JwtRegisteredClaimNames.Sub) != review.UserId)
                {
                    return Results.Forbid();
                }
                parameters.dbContext.Remove(review);
                await parameters.dbContext.SaveChangesAsync();
                return Results.NoContent();
            }).WithName("RemoveReview");
        }
        static IEnumerable<LinkDto> CreateLinks(int reviewId, HttpContext httpContext, LinkGenerator linkGenerator)
        {
            yield return new LinkDto(linkGenerator.GetUriByName(httpContext, "GetReview", new { reviewId }), "self", "GET");
            yield return new LinkDto(linkGenerator.GetUriByName(httpContext, "EditReview", new { reviewId }), "edit", "PUT");
            yield return new LinkDto(linkGenerator.GetUriByName(httpContext, "RemoveReview", new { reviewId }), "delete", "GET");
        }
    }
}
