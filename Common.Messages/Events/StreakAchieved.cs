namespace Common.Messages.Events;
public record StreakAchieved(Guid UserId, Guid HabitId, int StreakLength);