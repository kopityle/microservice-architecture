using HabitService.Domain.Entities;
using HabitService.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HabitService.Infrastructure.Persistence.Repositories;

public class HabitRepository : IHabitRepository
{
    private readonly HabitDbContext _context;

    public HabitRepository(HabitDbContext context)
    {
        _context = context;
    }

    // Тут мы реализуем все методы, обещанные в интерфейсе IHabitRepository

    public async Task<Habit?> GetByIdAsync(Guid id)
    {
        return await _context.Habits.FindAsync(id);
    }

    public async Task<IEnumerable<Habit>> GetAllAsync()
    {
        return await _context.Habits.ToListAsync();
    }

    public async Task AddAsync(Habit habit)
    {
        await _context.Habits.AddAsync(habit);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Habit habit)
    {
        _context.Habits.Update(habit);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var habit = await GetByIdAsync(id);
        if (habit != null)
        {
            _context.Habits.Remove(habit);
            await _context.SaveChangesAsync();
        }
    }
}