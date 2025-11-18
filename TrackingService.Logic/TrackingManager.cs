using Microsoft.EntityFrameworkCore;
using TrackingService.CoreLib;
using TrackingService.CoreLib.Interfaces;
using TrackingService.Dal;
using MassTransit;
using Common.Messages.Events;
using Common.Interfaces;

namespace TrackingService.Logic;

public class TrackingManager
{
    private readonly TrackingDbContext _context;
    private readonly IHabitServiceClient _habitServiceClient;
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly IDistributedSemaphore _semaphore; // 1. Поле для семафора

    // Конструктор: добавляем IDistributedSemaphore
    public TrackingManager(
        TrackingDbContext context,
        IHabitServiceClient habitServiceClient,
        IPublishEndpoint publishEndpoint,
        IDistributedSemaphore semaphore) // 2. Внедряем зависимость
    {
        _context = context;
        _habitServiceClient = habitServiceClient;
        _publishEndpoint = publishEndpoint;
        _semaphore = semaphore;
    }

    // CRUD с Семафором
    public async Task MarkHabitAsDone(Guid habitId, Guid userId)
    {
        // 3. Захватываем "турникет" (место в семафоре)
        // Если свободных мест нет, код остановится здесь и будет ждать
        await using (await _semaphore.WaitAsync())
        {
            // Проверяем, существует ли такая привычка в HabitService
            var habitExists = await _habitServiceClient.HabitExistsAsync(habitId, CancellationToken.None);
            if (!habitExists)
            {
                throw new Exception($"Привычка с ID {habitId} не найдена в HabitService.");
            }

            var today = DateOnly.FromDateTime(DateTime.UtcNow);

            // --- Работа с сущностью DailyRecord ---
            var existingRecord = await _context.DailyRecords
                .FirstOrDefaultAsync(r => r.HabitId == habitId && r.UserId == userId && r.Date == today);

            if (existingRecord != null)
            {
                return;
            }

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
            var streak = await _context.Streaks
                .FirstOrDefaultAsync(s => s.HabitId == habitId && s.UserId == userId);

            var yesterday = today.AddDays(-1);

            if (streak == null)
            {
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
                if (streak.LastUpdateDate == yesterday)
                {
                    streak.CurrentStreak++;
                }
                else if (streak.LastUpdateDate < yesterday)
                {
                    streak.CurrentStreak = 1;
                }

                if (streak.CurrentStreak > streak.LongestStreak)
                {
                    streak.LongestStreak = streak.CurrentStreak;
                }
                streak.LastUpdateDate = today;
                _context.Streaks.Update(streak);
            }

            if (streak.CurrentStreak == 7)
            {
                await _publishEndpoint.Publish(new StreakAchieved(userId, habitId, 7));
            }


            await _context.SaveChangesAsync();
        }
    }
}