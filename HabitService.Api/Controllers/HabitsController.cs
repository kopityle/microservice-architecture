using HabitService.Application.DTOs;
using HabitService.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace HabitService.Api.Controllers;

[ApiController]
[Route("api/habits")]
public class HabitsController : ControllerBase
{
    private readonly IHabitManagementService _habitService;

    public HabitsController(IHabitManagementService habitService)
    {
        _habitService = habitService;
    }

    // POST /api/habits
    [HttpPost]
    public async Task<IActionResult> CreateHabit([FromBody] CreateHabitRequest request)
    {
        await _habitService.CreateHabitAsync(request);
        return Ok();
    }

    // GET /api/habits/{id}
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetHabitById(Guid id)
    {
        // Вызываем метод из нашего сервиса, а не из репозитория
        var habit = await _habitService.GetHabitByIdAsync(id);

        if (habit == null)
        {
            // Если сервис вернул null, значит привычка не найдена
            return NotFound(); // Возвращаем ошибку 404
        }

        // Если привычка найдена, возвращаем её и статус 200 OK
        return Ok(habit);
    }
}