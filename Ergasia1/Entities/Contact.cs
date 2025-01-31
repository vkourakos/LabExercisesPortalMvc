namespace Ergasia1.Entities;

public class Contact
{
    public int Id { get; set; }
    public string UserId { get; set; } = null!;
    public string ContactId { get; set; } = null!;

    #region References
    public User User { get; set; }
    public User ContactUser { get; set; }
    #endregion
}

