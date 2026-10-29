using CastleGuardian.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace CastleGuardian.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Visitor> Visitors => Set<Visitor>();
    public DbSet<VisitorActionHistory> VisitorActionHistories => Set<VisitorActionHistory>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Visitor>()
            .HasMany(v => v.ActionHistory)
            .WithOne(h => h.Visitor)
            .HasForeignKey(h => h.VisitorId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}