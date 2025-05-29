using Ensek.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Ensek.Api.Data;

public class MeterReadingsDbContext(DbContextOptions<MeterReadingsDbContext> options) : DbContext(options)
{
    public DbSet<Account> Accounts { get; set; } = null!;
    public DbSet<MeterReading> MeterReadings { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<MeterReading>()
            .HasOne(m => m.Account)
            .WithMany(a => a.MeterReadings)
            .HasForeignKey(m => m.AccountId);
        
        base.OnModelCreating(modelBuilder);
    }
}