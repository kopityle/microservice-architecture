using HabitService.Domain.Entities;

namespace HabitService.Domain.Interfaces;

public interface ITagRepository
{
    Task AddAsync(Tag tag);
    Task<IEnumerable<Tag>> GetAllAsync();
}