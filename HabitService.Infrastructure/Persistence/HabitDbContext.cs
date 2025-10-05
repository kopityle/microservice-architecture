using Microsoft.EntityFrameworkCore;
using HabitService.Domain.Entities;

namespace HabitService.Infrastructure.Persistence;

public class HabitDbContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<Habit> Habits { get; set; }
    public DbSet<Tag> Tags { get; set; }

    public HabitDbContext(DbContextOptions<HabitDbContext> options) : base(options)
    {
    }
}