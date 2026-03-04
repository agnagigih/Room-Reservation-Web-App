using Microsoft.EntityFrameworkCore;
using RoomBooking.Models;

namespace RoomBooking.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Room> Rooms { get; set; }
        public DbSet<Booking> Bookings { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // User
            modelBuilder.Entity<User>(e => {
                e.HasIndex(u => u.Email).IsUnique();
                e.Property(u => u.Role).HasDefaultValue("User");
            });

            // Room
            modelBuilder.Entity<Room>(e => {
                e.HasIndex(r => r.Name).IsUnique();
            });

            // Booking
            modelBuilder.Entity<Booking>(e => {
                e.HasOne(b => b.User).WithMany(u => u.Bookings).HasForeignKey(b => b.UserId).OnDelete(DeleteBehavior.Restrict);
                e.HasOne(b => b.Room).WithMany(r => r.Bookings).HasForeignKey(b => b.RoomId).OnDelete(DeleteBehavior.Restrict);
                e.HasIndex(b => new { b.RoomId, b.BookingDate, b.StartTime });
            });

            // Seed Admin
            modelBuilder.Entity<User>().HasData(new User
            {
                Id = 1,
                FullName = "Administrator",
                Email = "admin@roombooking.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123"),
                Role = "Admin",
                IsActive = true,
                IsDeleted = false,
                CreatedAt = new DateTime(2026, 1, 1)
            });

            // Seed Rooms
            modelBuilder.Entity<Room>().HasData(
                new Room { Id = 1, Name = "Ruang Rapat Utama", Location = "Lantai 1", Capacity = 20, Description = "Ruang rapat besar dengan proyektor & AC", IsActive = true, CreatedAt = new DateTime(2026, 1, 1) },
                new Room { Id = 2, Name = "Ruang Meeting A", Location = "Lantai 2", Capacity = 10, Description = "Ruang meeting sedang dilengkapi TV 55 inch", IsActive = true, CreatedAt = new DateTime(2026, 1, 1) },
                new Room { Id = 3, Name = "Ruang Training", Location = "Lantai 3", Capacity = 50, Description = "Ruang training kapasitas besar", IsActive = true, CreatedAt = new DateTime(2026, 1, 1) },
                new Room { Id = 4, Name = "Ruang Direksi", Location = "Lantai 4", Capacity = 8, Description = "Ruang meeting eksklusif untuk direksi", IsActive = true, CreatedAt = new DateTime(2026, 1, 1) }
            );
        }
    }
}
