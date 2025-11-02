using MassTransit;
using Common.Messages.Events;
using System.Threading.Tasks;

namespace AchievementService.Api.Consumers;

public class StreakAchievedConsumer : IConsumer<StreakAchieved>
{
    private readonly ILogger<StreakAchievedConsumer> _logger;

    public StreakAchievedConsumer(ILogger<StreakAchievedConsumer> logger)
    {
        _logger = logger;
    }

    // Этот метод будет автоматически вызван, когда придет сообщение типа StreakAchieved
    public Task Consume(ConsumeContext<StreakAchieved> context)
    {
        var message = context.Message;

        // Наша бизнес-логика: просто выводим в лог, что получили сообщение
        _logger.LogInformation(
            "User {UserId} received an achievement for a {StreakLength}-day streak on habit {HabitId}!",
            message.UserId,
            message.StreakLength,
            message.HabitId
        );

        // В реальном проекте здесь был бы код для сохранения ачивки в базу данных

        return Task.CompletedTask;
    }
}