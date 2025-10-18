using HabitService.Application.DTOs;
using HabitService.Application.Interfaces;
using HabitService.Domain.Entities;
using HabitService.Domain.Interfaces;

namespace HabitService.Application.Services;

public class HabitManagementService : IHabitManagementService
{
    private readonly IHabitRepository _habitRepository;

    public HabitManagementService(IHabitRepository habitRepository)
    {
        _habitRepository = habitRepository;
    }

    public async Task CreateHabitAsync(CreateHabitRequest request)
    {
        var newHabit = new Habit
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Description = request.Description,
            UserId = request.UserId
        };

        await _habitRepository.AddAsync(newHabit);
    }

    public async Task<Habit?> GetHabitByIdAsync(Guid id)
    {
        // Вся логика работы с базой скрыта в Infrastructure слое.
        return await _habitRepository.GetByIdAsync(id);
    }
}