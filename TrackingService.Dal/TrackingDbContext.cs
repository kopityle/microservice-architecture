using Microsoft.EntityFrameworkCore;
using System.IO;
using TrackingService.CoreLib;

namespace TrackingService.Dal;

public class TrackingDbContext : DbContext
{
    public DbSet<DailyRecord> DailyRecords { get; set; }
    public DbSet<Streak> Streaks { get; set; }

    public TrackingDbContext(DbContextOptions<TrackingDbContext> options) : base(options)
    {
    }
}