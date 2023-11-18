using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.JsonWebTokens;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using webapi.Auth.Entities;

namespace webapi.Auth
{
    public static class AuthEndpoints
    {
        public static void AddAuthApi(this WebApplication app)
        {
            app.MapPost("api/register", async (UserManager<User> userManager, RegisterUserDto registerUserDto) =>
            {
                var user = await userManager.FindByNameAsync(registerUserDto.Username);
                if (user != null)
                {
                    return Results.UnprocessableEntity("Username already taken");
                }
                var newUser = new User
                {
                    Email = registerUserDto.Email,
                    UserName = registerUserDto.Username,
                };
                var createUserResult=await userManager.CreateAsync(newUser, registerUserDto.Password);
                if(!createUserResult.Succeeded)
                {
                    return Results.UnprocessableEntity();
                }
                await userManager.AddToRoleAsync(newUser, UserRoles.BasicUser);
                return Results.Created("api/login", new UserDto(newUser.Id, newUser.UserName, newUser.Email));
            });

            app.MapPost("api/login", async (UserManager<User> userManager, JwtTokenService jwtTokenService, LoginDto loginDto) =>
            {
                var user = await userManager.FindByNameAsync(loginDto.Username);
                if (user == null)
                {
                    return Results.UnprocessableEntity("Username or password was incorrect");
                }
                var isPasswordValid=await userManager.CheckPasswordAsync(user, loginDto.Password);
                if (!isPasswordValid)
                {
                    return Results.UnprocessableEntity("Username or password was incorrect");
                }

                user.ForceRelogin = false;
                await userManager.UpdateAsync(user);

                var roles=await userManager.GetRolesAsync(user);
                var accessToken = jwtTokenService.CreateAccessToken(user.UserName, user.Id, roles);
                var refreshToken = jwtTokenService.CreateRefreshToken(user.Id);

                return Results.Ok(new SuccessfulLoginDto(accessToken, refreshToken, user.Id));
            });

            app.MapPost("api/accessToken", async (UserManager<User> userManager, JwtTokenService jwtTokenService,  RefreshTokenStore refreshTokenStore, RefreshAccessTokenDto refreshAccessTokenDto) =>
            {
                if(!jwtTokenService.TryParseRefreshToken(refreshAccessTokenDto.RefreshToken, out var claims))
                {
                    return Results.UnprocessableEntity();
                }
                var userId = claims.FindFirstValue(JwtRegisteredClaimNames.Sub);
                var user=await userManager.FindByIdAsync(userId);
                if(user== null || user.ForceRelogin || refreshTokenStore.IsRefreshTokenRevoked(refreshAccessTokenDto.RefreshToken))
                {
                    return Results.UnprocessableEntity("Invalid token or user not logged in");
                }
                var roles = await userManager.GetRolesAsync(user);
                var accessToken = jwtTokenService.CreateAccessToken(user.UserName, user.Id, roles);
                var refreshToken = jwtTokenService.CreateRefreshToken(user.Id);
                return Results.Ok(new SuccessfulLoginDto(accessToken, refreshToken, user.Id));
            });

            app.MapPost("api/logout", async (UserManager<User> userManager, RefreshTokenStore refreshTokenStore, LogoutDto logoutDto) =>
            {
                var user = await userManager.FindByNameAsync(logoutDto.Username);
                if(user==null)
                {
                    return Results.UnprocessableEntity("User not found");
                }
                user.ForceRelogin = true;
                await userManager.UpdateAsync(user);
                refreshTokenStore.RevokeRefreshToken(logoutDto.RefreshToken);
                return Results.Ok("Logout successful");
            });
        }
    }
}
public record SuccessfulLoginDto(string AccessToken, string RefreshToken, string UserId);
public record RegisterUserDto(string Username, string Email, string Password);
public record UserDto(string UserId, string Email, string Username);
public record LoginDto(string Username, string Password);
public record RefreshAccessTokenDto(string RefreshToken);
public record LogoutDto(string Username, string RefreshToken);
