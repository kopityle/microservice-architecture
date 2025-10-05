using HabitService.Domain.Entities;

namespace HabitService.Domain.Interfaces;

public interface IHabitRepository
{
    Task<Habit?> GetByIdAsync(Guid id);
    Task<IEnumerable<Habit>> GetAllAsync();
    Task AddAsync(Habit habit);
    Task UpdateAsync(Habit habit);
    Task DeleteAsync(Guid id);
}