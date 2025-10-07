namespace TrackingService.CoreLib;

public class DailyRecord
{
    public Guid Id { get; set; }
    public Guid HabitId { get; set; }
    public Guid UserId { get; set; }
    public DateOnly Date { get; set; }
    public RecordStatus Status { get; set; }
}

public enum RecordStatus
{
    Done,
    Skipped
}