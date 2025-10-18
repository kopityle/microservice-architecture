using HabitService.Application.DTOs;
using HabitService.Domain.Entities;

namespace HabitService.Application.Interfaces;

public interface IHabitManagementService
{
    Task CreateHabitAsync(CreateHabitRequest request);
    Task<Habit?> GetHabitByIdAsync(Guid id);
}