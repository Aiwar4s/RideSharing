using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using webapi.Auth.Entities;
using webapi.Data.Entities;

namespace webapi.Data
{
    public class TripDbContext:IdentityDbContext<User>
    {
        private readonly IConfiguration _configuration;
        public DbSet<Driver> Drivers { get; set; }
        public DbSet<Trip> Trips { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public TripDbContext(IConfiguration configuration)
        {
            _configuration= configuration;
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseMySQL(_configuration.GetConnectionString("MySQL"));
        }
    }
}
