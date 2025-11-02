using MassTransit;
using Common.Messages.Events; // Для DeleteUserRequested
using Microsoft.AspNetCore.Mvc;

namespace HabitService.Api.Controllers;

[ApiController]
[Route("api/users")]
public class UsersController : ControllerBase
{
    private readonly IPublishEndpoint _publishEndpoint;

    public UsersController(IPublishEndpoint publishEndpoint)
    {
        _publishEndpoint = publishEndpoint;
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteUser(Guid id)
    {
        // Отправляем стартовое событие, которое "услышит" наш оркестратор
        await _publishEndpoint.Publish(new DeleteUserRequested(id));

        // Возвращаем 202 Accepted - "Запрос принят в обработку"
        return Accepted();
    }
}