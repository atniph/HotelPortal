using HotelPortal.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace HotelPortal.Data
{
    public class HotelPortalContext : IdentityDbContext<SiteUser, IdentityRole<int>, int>
    {
        public HotelPortalContext(DbContextOptions<HotelPortalContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder)
            builder.Entity<SiteUser>().ToTable("SiteUsers");
        }

        public DbSet<City> Cities { get; set; }

        public DbSet<Hotel> Hotels { get; set; }

        public DbSet<Room> Rooms { get; set; }

        public DbSet<Booking> Bookings { get; set; }

        public DbSet<Review> Reviews { get; set; }

        public DbSet<HotelImage> HotelImages { get; set; }
        public DbSet<RoomImage> RoomImages { get; set; }

    }
}
