namespace Ergasia1.Entities;

public class Group
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public string UserId { get; set; } = null!;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    #region Collections
    public List<User> Members { get; set; } = [];
    #endregion

    #region References
    public User Creator { get; set; }
    #endregion
}

