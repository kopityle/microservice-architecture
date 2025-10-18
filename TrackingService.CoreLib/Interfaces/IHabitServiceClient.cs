namespace TrackingService.CoreLib.Interfaces;

public interface IHabitServiceClient
{
    Task<bool> HabitExistsAsync(Guid habitId, CancellationToken cancellationToken);
}