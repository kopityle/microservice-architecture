namespace TrackingService.CoreLib;

public class Streak
{
    public Guid Id { get; set; }
    public Guid HabitId { get; set; }
    public Guid UserId { get; set; }
    public int CurrentStreak { get; set; }
    public int LongestStreak { get; set; }
    public DateOnly LastUpdateDate { get; set; }
}