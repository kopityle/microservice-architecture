using MassTransit;
using Common.Messages.Commands;
using Common.Messages.Events;
using System.Threading.Tasks;

namespace AchievementService.Api.Consumers;

public class DeleteUserAchievementsConsumer : IConsumer<DeleteUserAchievements>
{
    private readonly ILogger<DeleteUserAchievementsConsumer> _logger;

    public DeleteUserAchievementsConsumer(ILogger<DeleteUserAchievementsConsumer> logger)
    {
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<DeleteUserAchievements> context)
    {
        var userId = context.Message.UserId;
        _logger.LogInformation("Получена команда на удаление ачивок для пользователя {UserId}", userId);

        // --- Здесь была бы логика удаления из БД ---
        _logger.LogInformation("Ачивки для пользователя {UserId} успешно удалены", userId);

        // Сообщаем оркестратору, что этот шаг выполнен
        await context.Publish(new UserAchievementsDeleted(userId));
    }
}