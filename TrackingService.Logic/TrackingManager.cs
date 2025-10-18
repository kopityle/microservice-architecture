using Microsoft.EntityFrameworkCore;
using TrackingService.CoreLib;
using TrackingService.CoreLib.Interfaces;
using TrackingService.Dal;
using TrackingService.CoreLib.Interfaces;

namespace TrackingService.Logic;

public class TrackingManager
{
    private readonly TrackingDbContext _context;
    private readonly IHabitServiceClient _habitServiceClient;

    // Конструктор: Logic слой получает DbContext от Dal слоя
    public TrackingManager(TrackingDbContext context, IHabitServiceClient habitServiceClient)
    {
        _context = context;
        _habitServiceClient = habitServiceClient;
    }

    //  CRUD
    public async Task MarkHabitAsDone(Guid habitId, Guid userId)
    {
        // Проверяем, существует ли такая привычка в HabitService
        var habitExists = await _habitServiceClient.HabitExistsAsync(habitId, CancellationToken.None);
        if (!habitExists)
        {
            // Если привычки нет, мы не можем создать для нее отметку.
            // Бросаем исключение, чтобы сообщить об ошибке.
            throw new Exception($"Привычка с ID {habitId} не найдена в HabitService.");
        }

        var today = DateOnly.FromDateTime(DateTime.UtcNow);

        // --- Работа с сущностью DailyRecord ---

        // 1. Проверяем, может мы уже отмечали сегодня?
        var existingRecord = await _context.DailyRecords
            .FirstOrDefaultAsync(r => r.HabitId == habitId && r.UserId == userId && r.Date == today);

        if (existingRecord != null)
        {
            return; // Если уже отмечено, ничего не делаем
        }

        // 2. Если нет, создаем новую отметку
        var newRecord = new DailyRecord
        {
            Id = Guid.NewGuid(),
            HabitId = habitId,
            UserId = userId,
            Date = today,
            Status = RecordStatus.Done
        };
        await _context.DailyRecords.AddAsync(newRecord);


        // --- Работа с сущностью Streak ---

        // 3. Находим стрик для этой привычки
        var streak = await _context.Streaks
            .FirstOrDefaultAsync(s => s.HabitId == habitId && s.UserId == userId);

        var yesterday = today.AddDays(-1);

        if (streak == null)
        {
            // 4a. Если стрика нет, создаем новый
            streak = new Streak
            {
                Id = Guid.NewGuid(),
                HabitId = habitId,
                UserId = userId,
                CurrentStreak = 1,
                LongestStreak = 1,
                LastUpdateDate = today
            };
            await _context.Streaks.AddAsync(streak);
        }
        else
        {
            // 4b. Если стрик есть, обновляем его
            if (streak.LastUpdateDate == yesterday)
            {
                // Если последнее обновление было вчера, увеличиваем стрик
                streak.CurrentStreak++;
            }
            else if (streak.LastUpdateDate < yesterday)
            {
                // Если был пропуск, сбрасываем стрик до 1
                streak.CurrentStreak = 1;
            }
            // Если LastUpdateDate == today, значит мы уже сегодня отмечали, и вышли бы на шаге 1

            // Обновляем самый длинный стрик, если нужно
            if (streak.CurrentStreak > streak.LongestStreak)
            {
                streak.LongestStreak = streak.CurrentStreak;
            }
            streak.LastUpdateDate = today;
            _context.Streaks.Update(streak);
        }

        // 5. Сохраняем все изменения в базе данных ОДНОЙ транзакцией
        await _context.SaveChangesAsync();
    }
}