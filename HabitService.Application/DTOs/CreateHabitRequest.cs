﻿namespace HabitService.Application.DTOs;

public class CreateHabitRequest
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public Guid UserId { get; set; }
}