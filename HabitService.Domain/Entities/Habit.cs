namespace HabitService.Domain.Entities;

public class Habit
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; } // Описание может быть необязательным
    public Guid UserId { get; set; }
}