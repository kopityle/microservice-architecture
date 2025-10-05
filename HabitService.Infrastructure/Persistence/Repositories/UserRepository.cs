using HabitService.Domain.Entities;
using HabitService.Domain.Interfaces;

namespace HabitService.Infrastructure.Persistence.Repositories;

public class UserRepository : IUserRepository
{
    private readonly HabitDbContext _context;

    public UserRepository(HabitDbContext context)
    {
        _context = context;
    }

    public async Task<User?> GetByIdAsync(Guid id)
    {
        return await _context.Users.FindAsync(id);
    }

    public async Task AddAsync(User user)
    {
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();
    }
}