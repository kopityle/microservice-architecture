using HabitService.Application.DTOs;

namespace HabitService.Application.Interfaces;

public interface IHabitManagementService
{
    Task CreateHabitAsync(CreateHabitRequest request);
    // Тут будут другие методы: GetAll, GetById и т.д.
}