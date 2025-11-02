using AchievementService.CoreLib;
using AchievementService.Dal;

namespace AchievementService.Logic;

public class AchievementManager
{
    private readonly AchievementDbContext _context;

    public AchievementManager(AchievementDbContext context)
    {
        _context = context;
    }

    public async Task GrantAchievementAsync(Guid userId, string achievementName)
    {
        var alreadyExists = _context.Achievements.Any(a => a.UserId == userId && a.Name == achievementName);
        if (alreadyExists)
        {
            return;
        }

        var newAchievement = new Achievement
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Name = achievementName,
            DateReceived = DateTime.UtcNow
        };

        await _context.Achievements.AddAsync(newAchievement);
        await _context.SaveChangesAsync();
    }
}