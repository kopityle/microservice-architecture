using MassTransit;
using Common.Messages.Commands;
using Common.Messages.Events;
using TrackingService.Dal;
using Microsoft.EntityFrameworkCore;

namespace TrackingService.Api.Consumers;

public class DeleteUserTrackingDataConsumer : IConsumer<DeleteUserTrackingData>
{
    private readonly TrackingDbContext _dbContext;

    public DeleteUserTrackingDataConsumer(TrackingDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task Consume(ConsumeContext<DeleteUserTrackingData> context)
    {
        var userId = context.Message.UserId;

        // --- Настоящая логика удаления из БД ---
        var recordsToDelete = await _dbContext.DailyRecords.Where(r => r.UserId == userId).ToListAsync();
        var streaksToDelete = await _dbContext.Streaks.Where(s => s.UserId == userId).ToListAsync();

        _dbContext.DailyRecords.RemoveRange(recordsToDelete);
        _dbContext.Streaks.RemoveRange(streaksToDelete);

        await _dbContext.SaveChangesAsync();

        await context.Publish(new UserTrackingDataDeleted(userId));
    }
}