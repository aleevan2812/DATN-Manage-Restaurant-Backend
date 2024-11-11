using System.Reflection;
using Application.Common.Interfaces;
using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Infrastructure.Data;

public class ApplicationDbContext : DbContext, IApplicationDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<Account> Accounts => Set<Account>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<Guest> Guests => Set<Guest>();
    public DbSet<DishSnapshot> DishSnapshots => Set<DishSnapshot>();
    public DbSet<Table> Tables => Set<Table>();
    public DbSet<Dish> Dishes => Set<Dish>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    public DatabaseFacade Database => base.Database;

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        builder.Entity<DishSnapshot>()
            .HasOne(ds => ds.Dish)
            .WithMany(d => d.Snapshots)
            .HasForeignKey(ds => ds.DishId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}