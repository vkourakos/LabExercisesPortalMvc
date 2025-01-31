namespace Ergasia1.Entities;

public class Post
{
    public int Id { get; set; }
    public string Title { get; set; } = null!;
    public string Content { get; set; } = null!;
    public string Category { get; set; } = null!;
    public string UserId { get; set; } = null!;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    #region References
    public User User { get; set; }
    #endregion

    #region Collections
    public List<Tag> Tags { get; set; } = [];
    #endregion
}


