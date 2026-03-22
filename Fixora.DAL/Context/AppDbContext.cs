using Fixora.DAL.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Fixora.DAL.Context;

public class AppDbContext : IdentityDbContext<AppUser>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Building> Buildings => Set<Building>();
    public DbSet<Elevator> Elevators => Set<Elevator>();
    public DbSet<MaintenanceOrder> MaintenanceOrders => Set<MaintenanceOrder>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Building>(e =>
        {
            e.HasKey(b => b.Id);
            e.Property(b => b.Name).IsRequired().HasMaxLength(150);
            e.Property(b => b.Address).IsRequired().HasMaxLength(300);
        });

        builder.Entity<Elevator>(e =>
        {
            e.HasKey(el => el.Id);
            e.Property(el => el.Label).IsRequired().HasMaxLength(20);
            e.Property(el => el.SerialNumber).HasMaxLength(100);

            e.HasOne(el => el.Building)
             .WithMany(b => b.Elevators)
             .HasForeignKey(el => el.BuildingId)
             .OnDelete(DeleteBehavior.Restrict);
        });

        builder.Entity<MaintenanceOrder>(e =>
        {
            e.HasKey(o => o.Id);

            e.Property(o => o.MaintenanceType)
             .HasConversion<string>(); // stored as "Scheduled"/"Unscheduled" — readable in DB

            e.Property(o => o.ShortDescription).HasMaxLength(1000);

            e.HasOne(o => o.Elevator)
             .WithMany(el => el.MaintenanceOrders)
             .HasForeignKey(o => o.ElevatorId)
             .OnDelete(DeleteBehavior.Restrict);

            e.HasOne(o => o.AssignedEngineer)
             .WithMany(u => u.AssignedOrders)
             .HasForeignKey(o => o.AssignedEngineerId)
             .OnDelete(DeleteBehavior.Restrict);

            // Speeds up the monthly list query: filter by engineer + month
            e.HasIndex(o => new { o.AssignedEngineerId, o.ScheduledDate });
        });
    }
}

