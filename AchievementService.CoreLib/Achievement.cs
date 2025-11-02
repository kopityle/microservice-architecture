namespace AchievementService.CoreLib;

public class Achievement
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime DateReceived { get; set; }
}