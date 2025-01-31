﻿namespace Ergasia1.Entities;

public class Notification
{
    public int Id { get; set; }
    public string UserId { get; set; } = null!;
    public string Message { get; set; } = null!;
    public bool IsRead { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    #region References
    public User User { get; set; }
    #endregion
}

