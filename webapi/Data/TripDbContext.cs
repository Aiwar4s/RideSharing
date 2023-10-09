using Microsoft.EntityFrameworkCore;
using webapi.Data.Entities;

namespace webapi.Data
{
    public class TripDbContext:DbContext
    {
        private readonly IConfiguration _configuration;
        public DbSet<User> Users { get; set; }
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
