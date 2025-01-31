namespace Ergasia1.Entities;

public class Message
{
    public int Id { get; set; }
    public string SenderId { get; set; } = null!;
    public string? ReceiverId { get; set; }
    public int? GroupId { get; set; }
    public string Content { get; set; } = null!;
    public DateTime SentAt { get; set; } = DateTime.UtcNow;


    #region References
    public User Sender { get; set; }
    public User? Receiver { get; set; }
    public Group? Group { get; set; }
    #endregion
}
