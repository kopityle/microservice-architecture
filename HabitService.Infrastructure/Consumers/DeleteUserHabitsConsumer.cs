using Common.Messages.Commands;
using Common.Messages.Events;
using HabitService.Infrastructure.Persistence;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace HabitService.Infrastructure.Consumers;

public class DeleteUserHabitsConsumer : IConsumer<DeleteUserHabits>
{
    private readonly HabitDbContext _dbContext;

    public DeleteUserHabitsConsumer(HabitDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task Consume(ConsumeContext<DeleteUserHabits> context)
    {
        var userId = context.Message.UserId;

        var habitsToDelete = await _dbContext.Habits.Where(h => h.UserId == userId).ToListAsync();
        var userToDelete = await _dbContext.Users.FindAsync(userId);

        _dbContext.Habits.RemoveRange(habitsToDelete);
        if (userToDelete != null) _dbContext.Users.Remove(userToDelete);

        await _dbContext.SaveChangesAsync();

        await context.Publish(new UserHabitsDeleted(userId));
    }
}