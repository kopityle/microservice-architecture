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
        return Ok(); // Или можно вернуть Created() с ID созданной привычки
    }
}