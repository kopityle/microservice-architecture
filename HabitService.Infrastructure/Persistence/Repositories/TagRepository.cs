using HabitService.Domain.Entities;
using HabitService.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HabitService.Infrastructure.Persistence.Repositories;

public class TagRepository : ITagRepository
{
    private readonly HabitDbContext _context;

    public TagRepository(HabitDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Tag tag)
    {
        await _context.Tags.AddAsync(tag);
        await _context.SaveChangesAsync();
    }

    public async Task<IEnumerable<Tag>> GetAllAsync()
    {
        return await _context.Tags.ToListAsync();
    }
}