using Microsoft.EntityFrameworkCore;
using AchievementService.CoreLib;

namespace AchievementService.Dal;

public class AchievementDbContext : DbContext
{
    public DbSet<Achievement> Achievements { get; set; }

    public AchievementDbContext(DbContextOptions<AchievementDbContext> options) : base(options)
    {
    }
}