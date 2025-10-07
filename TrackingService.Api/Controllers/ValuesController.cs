using Microsoft.AspNetCore.Mvc;
using TrackingService.Logic;

namespace TrackingService.Api.Controllers;

[ApiController]
[Route("api/tracking")]
public class TrackingController : ControllerBase
{
    private readonly TrackingManager _trackingManager;

    public TrackingController(TrackingManager trackingManager)
    {
        _trackingManager = trackingManager;
    }

    // POST api/tracking/mark-done?habitId=...&userId=...
    [HttpPost("mark-done")]
    public async Task<IActionResult> MarkHabitAsDone(Guid habitId, Guid userId)
    {
        // Просто передаем параметры, полученные из запроса, в наш Logic слой
        await _trackingManager.MarkHabitAsDone(habitId, userId);

        // Возвращаем ответ "200 OK"
        return Ok();
    }
}