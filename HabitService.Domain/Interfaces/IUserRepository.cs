using HabitService.Domain.Entities;

namespace HabitService.Domain.Interfaces;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(Guid id);
    Task AddAsync(User user);
}