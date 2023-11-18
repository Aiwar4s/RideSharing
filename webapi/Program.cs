using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using webapi;
using webapi.Auth;
using webapi.Auth.Entities;
using webapi.Data;

JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<TripDbContext>();
builder.Services.AddValidatorsFromAssemblyContaining<Program>();
builder.Services.AddTransient<JwtTokenService>();
builder.Services.AddSingleton<RefreshTokenStore>();
builder.Services.AddScoped<AuthDbSeeder>();

builder.Services.AddIdentity<User, IdentityRole>()
    .AddEntityFrameworkStores<TripDbContext>().AddDefaultTokenProviders();
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters.ValidAudience = builder.Configuration["Jwt:ValidAudience"];
    options.TokenValidationParameters.ValidIssuer = builder.Configuration["Jwt:ValidIssuer"];
    options.TokenValidationParameters.IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Secret"]));
});
builder.Services.AddAuthorization();

var app = builder.Build();

app.AddDriverApi();
app.AddTripApi();
app.AddReviewApi();
app.AddAuthApi();

app.UseAuthentication();
app.UseAuthorization();

using var scope=app.Services.CreateScope();
var dbSeeder = scope.ServiceProvider.GetRequiredService<AuthDbSeeder>();
await dbSeeder.SeedAsync();

app.Run();
